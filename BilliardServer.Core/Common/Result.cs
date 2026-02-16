namespace BilliardServer.Core.Common
{
    public record Result<T>(bool IsSuccess, T? Value = default, string? Error = null)
    {
        public static Result<T> Ok(T value) => new(true, value);
        public static Result<T> Fail(string error) => new(false, Error: error);
    }

    public record Result(bool IsSuccess, string? Error = null)
    {
        public static Result Ok() => new(true);
        public static Result Fail(string error) => new(false, Error: error);
    }
}
