using CattleystWebPortal.Models.Apis;

namespace CattleystWebPortal.Interfaces
{
    public interface IApiService
    {
        Task<ApiResult<T>> GetAsync<T>(string route, params object[]? values);
        Task<ApiResult> DeleteAsync(string route, params object[]? values);
        Task<ApiResult<T>> PostAsJsonAsync<T>(string route, object? content = null);
        Task<ApiResult> PostAsJsonAsync(string route, object? content = null);
        Task<ApiResult<T>> PutAsJsonAsync<T>(string route, object content);
        Task<ApiResult> PutAsJsonAsync(string route, object content);
        Task<ApiResult<T>> PatchAsJsonAsync<T>(string route, object content);
        Task<ApiResult> PatchAsJsonAsync(string route, object content);
    }
}
