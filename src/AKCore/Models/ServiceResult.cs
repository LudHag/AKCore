namespace AKCore.Models;

public class ServiceResult
{
    public bool Success { get; init; }
    public string Message { get; init; }

    public static ServiceResult Ok(string message = null) => new() { Success = true, Message = message };

    public static ServiceResult Fail(string message) => new() { Success = false, Message = message };
}

public class ServiceResult<T> : ServiceResult
{
    public T Value { get; init; }

    public static ServiceResult<T> Ok(T value, string message = null) =>
        new() { Success = true, Value = value, Message = message };

    public static new ServiceResult<T> Fail(string message) =>
        new() { Success = false, Message = message };
}
