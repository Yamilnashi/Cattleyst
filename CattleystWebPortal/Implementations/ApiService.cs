using CattleystWebPortal.Interfaces;
using CattleystWebPortal.Models.Apis;
using System.Net;
using System.Text;

namespace CattleystWebPortal.Implementations
{
    public class ApiService : IApiService
    {
        private readonly ILogger<ApiService> _logger;
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiService(ILogger<ApiService> logger,
            IHttpClientFactory clientFactory,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _client = clientFactory.CreateClient("CattleystWebApi");
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<ApiResult<T>> GetAsync<T>(string route, Dictionary<string, object>? values = null) =>
            SendAsync<T>(HttpMethod.Get, FormatRoute(route, values));

        public Task<ApiResult> DeleteAsync(string route, Dictionary<string, object>? values = null) =>
            SendAsync<Unit>(HttpMethod.Delete, FormatRoute(route, values))
                .ContinueWith(t => (ApiResult)t.Result, TaskScheduler.Default);

        public Task<ApiResult<T>> PostAsJsonAsync<T>(string route, object? content = null) =>
            SendAsync<T>(HttpMethod.Post, route, content);

        public Task<ApiResult> PostAsJsonAsync(string route, object? content = null) =>
            SendAsync<Unit>(HttpMethod.Post, route, content)
                .ContinueWith(t => (ApiResult)t.Result, TaskScheduler.Default);

        public Task<ApiResult<T>> PutAsJsonAsync<T>(string route, object content) =>
            SendAsync<T>(HttpMethod.Put, route, content);

        public Task<ApiResult> PutAsJsonAsync(string route, object content) =>
            SendAsync<Unit>(HttpMethod.Put, route, content)
            .ContinueWith(t => (ApiResult)t.Result, TaskScheduler.Default);

        public Task<ApiResult<T>> PatchAsJsonAsync<T>(string route, object content) =>
            SendAsync<T>(HttpMethod.Patch, route, content);

        public Task<ApiResult> PatchAsJsonAsync(string route, object content) =>
            SendAsync<Unit>(HttpMethod.Patch, route, content)
                .ContinueWith(t => (ApiResult)t.Result, TaskScheduler.Default);


        #region Private
        private async Task<ApiResult<T>> SendAsync<T>(HttpMethod method, string route, object? content = null, 
            CancellationToken cancellationToken = default)
        {
            HttpRequestMessage request = new HttpRequestMessage(method, route);

            if (method == HttpMethod.Post || 
                method == HttpMethod.Put || 
                method == HttpMethod.Delete || 
                method == HttpMethod.Patch)
            {
                string? idempotencyKey = _httpContextAccessor.HttpContext?.Request.Headers["Idempotency-Key"].FirstOrDefault();
                if (!string.IsNullOrEmpty(idempotencyKey))
                {
                    request.Headers.Add("Idempotency-Key", idempotencyKey);
                    _logger.LogDebug("Forwarded Idempotency-Key {Key} to {Method} request for {Route}", idempotencyKey, method, route);
                } else
                {
                    _logger.LogWarning("No Idempotency-Key header found in context for {Method} {Route}-proceeding without", method, route);
                }
            }

            if (content != null)
            {
                request.Content = JsonContent.Create(content, content.GetType());
            }

            try
            {
                HttpResponseMessage? response = await _client
                    .SendAsync(request, cancellationToken)
                    .ConfigureAwait(false);
                var headers = response.Headers.ToDictionary(x => x.Key, h => h.Value, StringComparer.OrdinalIgnoreCase);

                string? responseBody = null;
                if (response.Content.Headers.ContentLength > 0)
                {
                    responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                }

                if (response.IsSuccessStatusCode)
                {
                    if (typeof(T) == typeof(Unit) || 
                        response.StatusCode == HttpStatusCode.NoContent)
                    {
                        ApiResult successResult = ApiResult.Success((int)response.StatusCode, headers);
                        return (ApiResult<T>)(object)successResult;
                    }

                    var data = await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
                    return ApiResult<T>.Success(data!, (int)response.StatusCode, headers);
                } else
                {
                    var errorMessage = responseBody ?? response.ReasonPhrase ?? "Unknown Error";
                    _logger.LogWarning("API call failed: {Method} {Route} -> {StatusCode} {ReasonPhrase}\n{Body}", 
                        method, route, (int)response.StatusCode, response.ReasonPhrase, responseBody);

                    return ApiResult<T>.Failure(errorMessage, (int)response.StatusCode, headers);
                }                   
            } catch (Exception ex) when (ex is not TaskCanceledException)
            {
                _logger.LogError(ex, "Exception during API call: {Method} {Route}", method, route);
                return ApiResult<T>.Failure(ex.Message, 0);
            }
        }

        private string FormatRoute(string route, Dictionary<string, object>? values)
        {
            if (values == null)
            {
                return route;
            }

            string CombineListIntoQueryString(string key, Array list)
            {
                StringBuilder qs = new();
                foreach (object obj in list)
                {
                    qs.Append($"{key}={obj ?? string.Empty}");
                }
                return qs.ToString();
            };

            StringBuilder fullQs = new();
            foreach (KeyValuePair<string, object> kvp in values)
            {
                if (kvp.Value is Array list)
                {
                    fullQs.Append(CombineListIntoQueryString(kvp.Key, list));
                }                
            }
            return $"{route}?{fullQs.ToString()}";
        }
        #endregion

    }
}
