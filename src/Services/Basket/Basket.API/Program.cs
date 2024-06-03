using BuildingBlocks.Exceptions.Handler;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
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
builder.Services.AddExceptionHandler<CustomExceptionHandler>();
var app = builder.Build();

// Configure the HTTP request pipeline
app.MapCarter();
app.UseExceptionHandler(options => { });
app.Run();