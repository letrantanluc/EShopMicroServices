namespace BuildingBlocks.Exceptions;

public class InternalServerException : Exception
{
    public InternalServerException(string message) : base(message)
    {
    }

    public InternalServerException(string message, string details) : base(message)
    {
        Details = details;
    }

    public string? Details { get; }
}

/*
 * InternalServerException là một ngoại lệ (exception) thường được sử dụng để biểu thị rằng đã xảy ra lỗi bên trong máy chủ mà không thể xử lý được bằng cách thông thường.
 * Ngoại lệ này thường liên quan đến lỗi hệ thống, lỗi không mong muốn hoặc các tình huống mà máy chủ không thể tiếp tục xử lý yêu cầu.
 * */