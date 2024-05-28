namespace BuildingBlocks.Exceptions;

public class NotFoundException : Exception
{
    // throw new NotFoundException("Custom not found message");
    // in chuỗi câu lệnh chi tiết không theo định dạng cố định
    public NotFoundException(string message) : base(message)
    {
    }

    // throw new NotFoundException("User", 42);
    // Sử dụng khi bạn cần một thông báo lỗi nhất quán và theo định dạng cố định, đặc biệt khi bạn có thông tin về tên thực thể và khóa của nó.
    public NotFoundException(string name, object key) : base($"Entity \"{name}\" ({key}) was not found.")
    {
    }
}