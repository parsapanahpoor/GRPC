using GrpcDemo.Server.Interceptors;
using GrpcDemo.Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<CorrelationInterceptor>();
builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<CorrelationInterceptor>();
});

var app = builder.Build();

app.MapGrpcService<AccountGrpcService>();
app.MapGet("/", () =>
    "gRPC Server is running. Open GrpcDemo.Client (Blazor) at http://localhost:5200");

app.Run();
