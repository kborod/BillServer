namespace BilliardServer.Core.Common
{
    public record Result<T>(bool IsSuccess, T? Value = default, string? Error = null)
    {
        public static Result<T> Ok(T value) => new(true, value);
        public static Result<T> Fail(string error) => new(false, Error: error);
    }
}
