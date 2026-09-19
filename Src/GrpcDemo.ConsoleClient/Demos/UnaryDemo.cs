using Grpc.Core;
using GrpcDemo.Contracts;

namespace GrpcDemo.ConsoleClient.Demos;

/// <summary>
/// Unary: one request -> one response
/// Includes Metadata (Headers) and Deadline
/// </summary>
public static class UnaryDemo
{
    public static async Task RunAsync(AccountService.AccountServiceClient client)
    {
        Console.WriteLine();
        Console.WriteLine("=== Unary: GetBalance ===");

        // 1) Headers = Metadata (not part of protobuf body)
        var correlationId = Guid.NewGuid().ToString("N")[..8];
        var headers = new Metadata
        {
            { "x-correlation-id", correlationId }
        };

        // 2) Deadline = call time limit
        var deadline = DateTime.UtcNow.AddSeconds(5);

        Console.WriteLine($"Header  -> x-correlation-id = {correlationId}");
        Console.WriteLine("Deadline -> 5 seconds");
        Console.WriteLine("Body    -> account_number = 1001");

        using var call = client.GetBalanceAsync(
            new GetBalanceRequest { AccountNumber = "1001" },
            headers: headers,
            deadline: deadline);

        // 3) Single response
        var response = await call.ResponseAsync;

        // 4) Trailers = end-of-call metadata (from server interceptor)
        var trailers = call.GetTrailers();

        Console.WriteLine($"Body    <- balance = {response.Balance:N0} {response.Currency}");
        Console.WriteLine($"Trailer <- x-correlation-id = {trailers.GetValue("x-correlation-id")}");
        Console.WriteLine($"Trailer <- x-processed-by   = {trailers.GetValue("x-processed-by")}");
    }
}
