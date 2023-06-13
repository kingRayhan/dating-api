namespace api.Shared;

public class BaseResponse<T>
{
    public string message { get; set; }
    public int statusCode { get; set; }
    public T? data { get; set; }
}