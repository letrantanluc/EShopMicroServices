using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

//------------------- Add services to the container. ---------------------------
var assembly = typeof(Program).Assembly;
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

builder.Services.AddValidatorsFromAssembly(assembly);
// thư viện giúp đơn giản hóa việc xây dựng các API REST
builder.Services.AddCarter();

// thư viện quản lý dữ liệu NoSQL cho PostgreSQL
builder.Services.AddMarten(opts =>
{
    opts.Connection(builder.Configuration.GetConnectionString("Database")!);
    /*
     *  Cấu hình Marten để sử dụng lightweight sessions,
     *  loại session này nhẹ nhàng và hiệu quả cho các thao tác đọc và ghi dữ liệu thông thường mà không cần quản lý transaction một cách nghiêm ngặt.
     * */
}).UseLightweightSessions();

if (builder.Environment.IsDevelopment())
    builder.Services.InitializeMartenWith<CatalogInitialData>();

builder.Services.AddExceptionHandler<CustomExceptionHandler>();
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Database")!);

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