using Grpc.Core;
using Grpc.Core.Interceptors;

namespace GrpcDemo.Server.Interceptors;

/// <summary>
/// شبیه Middleware: خواندن Headers و نوشتن Trailers برای همهٔ Unaryها.
/// </summary>
public class CorrelationInterceptor : Interceptor
{
    private readonly ILogger<CorrelationInterceptor> _logger;

    public CorrelationInterceptor(ILogger<CorrelationInterceptor> logger)
    {
        _logger = logger;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var correlationId = context.RequestHeaders.GetValue("x-correlation-id") ?? "n/a";
        _logger.LogInformation(
            "gRPC {Method} | header x-correlation-id={CorrelationId}",
            context.Method,
            correlationId);

        try
        {
            var response = await continuation(request, context);

            // Trailers = متادیتای پایان تماس (شبیه headerهای انتهای پاسخ)
            context.ResponseTrailers.Add("x-correlation-id", correlationId);
            context.ResponseTrailers.Add("x-processed-by", "account-service");
            return response;
        }
        catch (RpcException ex)
        {
            _logger.LogWarning(
                "gRPC {Method} خطا | {StatusCode} | {Detail}",
                context.Method,
                ex.StatusCode,
                ex.Status.Detail);
            throw;
        }
    }
}
