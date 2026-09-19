using Grpc.Core;
using GrpcDemo.Contracts;

namespace GrpcDemo.ConsoleClient.Demos;

/// <summary>
/// Server Streaming: یک درخواست → چند پاسخ پشت‌سرهم از سرور
/// </summary>
public static class ServerStreamingDemo
{
    public static async Task RunAsync(AccountService.AccountServiceClient client)
    {
        Console.WriteLine();
        Console.WriteLine("=== Server Streaming: StreamTransactions ===");
        Console.WriteLine("یک درخواست می‌فرستیم؛ سرور چند Transaction با Delay برمی‌گرداند.");
        Console.WriteLine();

        var request = new StreamTransactionsRequest
        {
            AccountNumber = "1001",
            Limit = 4
        };

        // باز کردن استریم سمت سرور
        using var call = client.StreamTransactions(request);

        // خواندن پیام‌ها یکی‌یکی تا استریم تمام شود
        await foreach (var tx in call.ResponseStream.ReadAllAsync())
        {
            Console.WriteLine(
                $"  ← [{tx.Type}] {tx.Amount:N0} — {tx.Description}");
        }

        Console.WriteLine("استریم سرور تمام شد.");
    }
}
