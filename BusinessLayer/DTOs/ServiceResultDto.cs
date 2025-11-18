namespace BusinessLayer.DTOs
{
    public class ServiceResultDto<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

        public static ServiceResultDto<T> SuccessResult(T data, string message = "Thành công")
        {
            return new ServiceResultDto<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static ServiceResultDto<T> FailureResult(string message, List<string>? errors = null)
        {
            return new ServiceResultDto<T>
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }

        public static ServiceResultDto<T> FailureResult(string message, string error)
        {
            return new ServiceResultDto<T>
            {
                Success = false,
                Message = message,
                Errors = new List<string> { error }
            };
        }
    }
}
