using System.Collections.Generic;

namespace Chap10.Dtos;

public class APIResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public IEnumerable<string>? Errors { get; set; }

    public static APIResponse<T> Ok(T data, string? message = null)
    {
        return new APIResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static APIResponse<T> Fail(string message, IEnumerable<string>? errors = null)
    {
        return new APIResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors
        };
    }
}
