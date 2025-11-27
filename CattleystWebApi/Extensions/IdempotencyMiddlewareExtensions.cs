using CattleystWebApi.Middleware;

namespace CattleystWebApi.Extensions
{
    public static class IdempotencyMiddlewareExtensions
    {
        public static IApplicationBuilder UseIdempotency(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<IdempotencyMiddleware>();
        }
    }
}
