using CattleystData.Interfaces;
using CattleystData.Models.Enums;
using CattleystData.Models.Idempotency;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using System.Transactions;

namespace CattleystWebApi.Middleware
{
    public class IdempotencyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<IdempotencyMiddleware> _logger;
        private readonly string _connectionString;
        public IdempotencyMiddleware(ILogger<IdempotencyMiddleware> logger,
            IServiceScopeFactory scopeFactory,
            RequestDelegate next,
            IConfiguration config)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _next = next;
            _connectionString = config.GetConnectionString("dbCattleyst")!;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!HttpMethods.IsPost(context.Request.Method) &&
                !HttpMethods.IsPut(context.Request.Method) &&
                !HttpMethods.IsPatch(context.Request.Method) &&
                !HttpMethods.IsDelete(context.Request.Method))
            {
                await _next(context);
                return; // we don't care about non-mutable methods
            }

            using (IServiceScope scope = _scopeFactory.CreateScope())
            {
                IIdpyDbReadContext dbRead = scope.ServiceProvider.GetRequiredService<IIdpyDbReadContext>();
                IIdpyDbWriteContext dbWrite = scope.ServiceProvider.GetRequiredService<IIdpyDbWriteContext>();

                string? idempotencyKey = context.Request.Headers["Idempotency-Key"]
                .FirstOrDefault();

                if (string.IsNullOrEmpty(idempotencyKey) ||
                    !Guid.TryParse(idempotencyKey, out Guid keyGuid))
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync("Missing or invalid Idempotency-Key header.");
                    return;
                }

                context.Request.EnableBuffering();
                string requestBody = await ReadStreamAsync(context.Request.Body);
                context.Request.Body.Position = 0; // need to set back the position when it reaches the controller end point

                string requestHash = ComputeSHA256($"{context.Request.Method}{context.Request.Path}{requestBody}");

                IdempotencyRequest? existingRequest = null;

                IdempotencyRequest newRequest = new IdempotencyRequest
                {
                    RequestId = keyGuid,
                    RequestStateCode = ERequestState.Pending,
                    RequestHash = requestHash,
                    SavedDate = DateTime.UtcNow
                };

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (TransactionScope transactionScope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
                    {
                        IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TimeSpan.FromSeconds(30)
                    }))
                    {
                        using SqlTransaction dbTrans = connection.BeginTransaction();  // Sync for .NET Standard compat
                        try
                        {                           
                            existingRequest = await dbRead.IdempotencyRequestGet(keyGuid, connection, dbTrans);
                            if (existingRequest != null)
                            {
                                if (existingRequest.RequestHash != requestHash)
                                {
                                    _logger.LogWarning("Idempotency key {key} reused with mismatched hash.", keyGuid);
                                    context.Response.StatusCode = StatusCodes.Status409Conflict;
                                    await context.Response.WriteAsync("Idempotency key reused with mismatched payload. Use a new key for changes.");
                                    return;
                                }

                                if (existingRequest.RequestStateCode == ERequestState.Completed)
                                {
                                    context.Response.StatusCode = existingRequest.StatusCode ?? 200;
                                    if (!string.IsNullOrEmpty(existingRequest.ResponseJson))
                                    {
                                        context.Response.ContentType = "application/json";
                                        await context.Response.WriteAsync(existingRequest.ResponseJson);
                                    }
                                    return;
                                }
                                else if (existingRequest.RequestStateCode == ERequestState.Pending)
                                {
                                    context.Response.StatusCode = StatusCodes.Status409Conflict;
                                    await context.Response.WriteAsync("Request in progress.");
                                    return;
                                }
                                else if (existingRequest.RequestStateCode == ERequestState.Failed)
                                {
                                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                                    await context.Response.WriteAsync("Previous request failed; retry with new key.");
                                    return;
                                }
                            }                            
                            
                            await dbWrite.IdempotencyRequestAdd(newRequest.RequestId, (byte)newRequest.RequestStateCode,
                                newRequest.RequestHash, newRequest.SavedDate, connection, dbTrans);

                            var originalBody = context.Response.Body;
                            await using var responseBody = new MemoryStream();
                            context.Response.Body = responseBody;

                            await _next(context);  // Controller joins trans if using same conn string

                            responseBody.Seek(0, SeekOrigin.Begin);
                            string responseJson = await ReadStreamAsync(responseBody);
                            if (string.IsNullOrEmpty(responseJson))
                            {
                                responseJson = JsonConvert.SerializeObject(new { });
                            }

                            // Update completed with shared conn/trans
                            newRequest.ResponseJson = responseJson;
                            newRequest.StatusCode = context.Response.StatusCode;
                            newRequest.RequestStateCode = ERequestState.Completed;

                            await dbWrite.IdempotencyRequestUpdate(newRequest.RequestId, newRequest.ResponseJson,
                                newRequest.StatusCode, (byte)newRequest.RequestStateCode, connection, dbTrans);

                            dbTrans.Commit();  // Commit db trans
                            transactionScope.Complete();  // Commit scope

                            responseBody.Seek(0, SeekOrigin.Begin);
                            await responseBody.CopyToAsync(originalBody);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Idempotency processing failed—rolling back.");
                            // mark failed (if possible before rollback)
                            if (newRequest != null)
                            {
                                newRequest.RequestStateCode = ERequestState.Failed;
                                await dbWrite.IdempotencyRequestUpdate(newRequest.RequestId, null, null, (byte)newRequest.RequestStateCode, connection, dbTrans);
                            }
                            throw;  // scope/dbTrans rollback on dispose
                        }
                    }
                }

            }     
        }

        private static async Task<string> ReadStreamAsync(Stream stream)
        {
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true))
            {
                return await reader.ReadToEndAsync();
            }
        }

        private static string ComputeSHA256(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(bytes);
                return BitConverter
                    .ToString(hashBytes)
                    .Replace("-", "")
                    .ToLowerInvariant();
            }
        }

    }
}
