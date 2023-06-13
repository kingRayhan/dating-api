namespace api.DTOs
{
    public class BaseResponseDto<T>
    {
        public string message { get; set; }
        public int statusCode { get; set; }
        public T? data { get; set; }
    }
}