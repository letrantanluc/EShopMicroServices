namespace BuildingBlocks.Exceptions;

public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message)
    {
    }

    public BadRequestException(string message, string details) : base(message)
    {
        Details = details;
    }

    public string? Details { get; }
}

/*
 * BadRequestException là một ngoại lệ (exception) tùy chỉnh trong C# thường được sử dụng để biểu thị rằng một yêu cầu từ phía client (người dùng hoặc ứng dụng khác) là không hợp lệ.
 * Ngoại lệ này thường được sử dụng khi yêu cầu không tuân thủ các quy tắc hoặc điều kiện cần thiết để máy chủ có thể xử lý nó.
 * */