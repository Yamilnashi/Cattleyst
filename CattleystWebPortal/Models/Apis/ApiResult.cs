namespace CattleystWebPortal.Models.Apis
{
    public record ApiResult<T> : ApiResult
    {
        public T? Data { get; set; }
        public static ApiResult<T> Success(T data, int statusCode, IReadOnlyDictionary<string, IEnumerable<string>>? headers = null) =>
            new()
            {
                IsSuccess = true,
                Data = data,
                StatusCode = statusCode,
                Headers = headers
            };

        public new static ApiResult<T> Failure(string errorMessage, int statusCode, IReadOnlyDictionary<string, IEnumerable<string>>? headers = null) =>
            new()
            {
                IsSuccess = false,
                ErrorMessage = errorMessage,
                StatusCode = statusCode,
                Headers = headers
            };

    }

    public record ApiResult
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public int StatusCode { get; set; }
        public IReadOnlyDictionary<string, IEnumerable<string>>? Headers { get; set; }
        public static ApiResult Success(int statusCode, IReadOnlyDictionary<string, IEnumerable<string>>? headers = null) =>
            new()
            {
                IsSuccess = true,
                StatusCode = statusCode,
                Headers = headers
            };

        public static ApiResult Failure(string errorMessage, int statusCode, IReadOnlyDictionary<string, IEnumerable<string>>? headers = null) =>
            new()
            {
                IsSuccess = false,
                ErrorMessage = errorMessage,
                StatusCode = statusCode,
                Headers = headers
            };

    }

    public readonly struct Unit { } // empty Unit class to send with api request to indicate we dont need a response back

}
