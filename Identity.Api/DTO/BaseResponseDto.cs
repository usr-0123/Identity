namespace Identity.Api.DTO;

public class BaseResponseDto
{
}

public class ResponseDto<T> : BaseResponseDto
{
    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; }
    public T? Data { get; set; }
}