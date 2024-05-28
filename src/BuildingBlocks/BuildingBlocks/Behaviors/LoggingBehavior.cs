using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BuildingBlocks.Behaviors;

public class LoggingBehavior<TRequest, TResponse>
    (ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IRequest<TResponse>
    where TResponse : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Đăng nhập thông tin bắt đầu xử lý yêu cầu, bao gồm tên kiểu yêu cầu (TRequest), tên kiểu phản hồi (TResponse), và dữ liệu yêu cầu (request).
        logger.LogInformation("[START] Handle request={Request} - Response={Response} - RequestData={RequestData}",
            typeof(TRequest).Name, typeof(TResponse).Name, request);

        // Tạo một Stopwatch để đo thời gian xử lý yêu cầu.
        var timer = new Stopwatch();
        timer.Start();

        // Gọi handler tiếp theo hoặc hành vi tiếp theo trong pipeline và chờ kết quả.
        var response = await next();

        // Dừng Stopwatch và lấy thời gian đã trôi qua (Elapsed).
        timer.Stop();
        var timeTaken = timer.Elapsed;

        // Nếu thời gian xử lý lớn hơn 3 giây, ghi nhật ký cảnh báo với thông tin về yêu cầu và thời gian xử lý.
        if (timeTaken.Seconds > 3) // if the request is greater than 3 seconds, then log the warnings
            logger.LogWarning("[PERFORMANCE] The request {Request} took {TimeTaken} seconds.",
                typeof(TRequest).Name, timeTaken.Seconds);

        // Đăng nhập thông tin kết thúc xử lý yêu cầu, bao gồm tên kiểu yêu cầu (TRequest) và tên kiểu phản hồi (TResponse).
        logger.LogInformation("[END] Handled {Request} with {Response}", typeof(TRequest).Name, typeof(TResponse).Name);
        return response;
    }
}