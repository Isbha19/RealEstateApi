public class GeneralResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }

    // Default constructor
    public GeneralResponse() { }

    // Constructor with success and message
    public GeneralResponse(bool success, string message)
    {
        Success = success;
        Message = message;
    }

    // Constructor with success, message, and data
    public GeneralResponse(bool success, string message, T data)
    {
        Success = success;
        Message = message;
        Data = data;
    }

    // Constructor with success and data only
    public GeneralResponse(bool success, T data)
    {
        Success = success;
        Data = data;
    }
}
