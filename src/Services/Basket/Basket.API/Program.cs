using BuildingBlocks.Exceptions.Handler;
using Discount.Grpc;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container

// Appication Services
var assembly = typeof(Program).Assembly;
builder.Services.AddCarter();
// thư viện hỗ trợ thiết kế theo mô hình CQRS
builder.Services.AddMediatR(config =>
{
    /*
     * Chỉ định rằng MediatR sẽ tìm kiếm và đăng ký tất cả các request và handler trong assembly chứa Program class (thường là assembly của ứng dụng hiện tại).
     * Điều này giúp tự động hóa quá trình đăng ký các dịch vụ cần thiết cho MediatR.
     * */
    config.RegisterServicesFromAssembly(assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});

//Data Services
builder.Services.AddMarten(opts =>
{
    opts.Connection(builder.Configuration.GetConnectionString("Database")!);
    opts.Schema.For<ShoppingCart>().Identity(x => x.UserName);
}).UseLightweightSessions();
builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.Decorate<IBasketRepository, CachedBasketRepository>();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    //options.InstanceName = "Basket";
});

// Grpc Services
builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(options =>
{
    options.Address = new Uri(builder.Configuration["GrpcSettings:DiscountUrl"]!);
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback =
        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    };

    return handler;
});

// Cross-Cutting Services
builder.Services.AddExceptionHandler<CustomExceptionHandler>();
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Database")!)
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!);
var app = builder.Build();

// Configure the HTTP request pipeline
app.MapCarter();
app.UseExceptionHandler(options => { });

app.UseHealthChecks("/health",
    new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });
app.Run();

/* builder.Services.Decorate<IBasketRepository, CachedBasketRepository>();
 Dòng mã này sử dụng Scrutor để áp dụng Decorator Pattern.
Decorate chỉ định rằng mỗi khi có một yêu cầu tới IBasketRepository, CachedBasketRepository sẽ được sử dụng để bao bọc BasketRepository.
Điều này có nghĩa là CachedBasketRepository sẽ được tạo ra và nó sẽ gọi BasketRepository khi cần thiết.
CachedBasketRepository có thể thêm các logic bổ sung như caching vào các phương thức của BasketRepository.
 */

/* builder.Services.AddStackExchangeRedisCache
Dòng mã này đăng ký và cấu hình Redis Cache trong DI container.
AddStackExchangeRedisCache là một phương thức mở rộng giúp bạn cấu hình Redis Cache dễ dàng.
options.Configuration lấy chuỗi kết nối tới Redis từ cấu hình ứng dụng. builder.Configuration.GetConnectionString("Redis") tìm chuỗi kết nối Redis từ file cấu hình (ví dụ như appsettings.json).
options.InstanceName có thể được sử dụng để đặt tên cho instance của Redis Cache. Dòng này đang bị comment, nếu cần bạn có thể bỏ comment và đặt tên cho instance.
 * */