using Grpc.Core;
using GrpcDemo.Contracts;

namespace GrpcDemo.ConsoleClient.Demos;

/// <summary>
/// Server Streaming: one request -> many responses from server
/// </summary>
public static class ServerStreamingDemo
{
    public static async Task RunAsync(AccountService.AccountServiceClient client)
    {
        Console.WriteLine();
        Console.WriteLine("=== Server Streaming: StreamTransactions ===");
        Console.WriteLine("Sending one request; server returns transactions with delay.");
        Console.WriteLine();

        var request = new StreamTransactionsRequest
        {
            AccountNumber = "1001",
            Limit = 4
        };

        using var call = client.StreamTransactions(request);

        await foreach (var tx in call.ResponseStream.ReadAllAsync())
        {
            Console.WriteLine(
                $"  <- [{tx.Type}] {tx.Amount:N0} - {tx.Description}");
        }

        Console.WriteLine("Server stream completed.");
    }
}
