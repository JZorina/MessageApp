namespace MessageQueues.Models
{
    public class ApiResponse<T>
    {
        public ApiResponse(int _statusCode, object _data)
        {
            StatusCode = _statusCode;
            Data = _data;
        }
        public int StatusCode { get; set; }
        public object Data { get; set; }
    }
}
