using GrpcDemo.Contracts;

namespace GrpcDemo.ConsoleClient.Demos;

/// <summary>
/// Client Streaming: many client requests -> one final server response
/// Important: call CompleteAsync after the last Write
/// </summary>
public static class ClientStreamingDemo
{
    public static async Task RunAsync(AccountService.AccountServiceClient client)
    {
        Console.WriteLine();
        Console.WriteLine("=== Client Streaming: DepositBatch ===");
        Console.WriteLine("Sending several deposits; then receiving one summary.");
        Console.WriteLine();

        using var call = client.DepositBatch();

        double[] amounts = [500_000, 250_000, 100_000];

        foreach (var amount in amounts)
        {
            await call.RequestStream.WriteAsync(new DepositRequest
            {
                AccountNumber = "1001",
                Amount = amount
            });

            Console.WriteLine($"  -> deposit sent: {amount:N0}");
            await Task.Delay(1500); // visible pacing for the presentation
        }

        await call.RequestStream.CompleteAsync();
        Console.WriteLine("  -> CompleteAsync()");

        var result = await call;
        Console.WriteLine($"  <- summary: count={result.Count} | total={result.TotalAmount:N0}");
    }
}
