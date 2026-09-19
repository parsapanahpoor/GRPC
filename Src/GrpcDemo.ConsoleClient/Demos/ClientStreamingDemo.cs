using GrpcDemo.Contracts;

namespace GrpcDemo.ConsoleClient.Demos;

/// <summary>
/// Client Streaming: چند درخواست از کلاینت → یک پاسخ نهایی از سرور
/// نکته مهم: بعد از آخرین Write حتماً CompleteAsync
/// </summary>
public static class ClientStreamingDemo
{
    public static async Task RunAsync(AccountService.AccountServiceClient client)
    {
        Console.WriteLine();
        Console.WriteLine("=== Client Streaming: DepositBatch ===");
        Console.WriteLine("چند واریز می‌فرستیم؛ آخر کار یک خلاصه می‌گیریم.");
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

            Console.WriteLine($"  → واریز ارسال شد: {amount:N0}");
            await Task.Delay(1500); // فقط برای دیده شدن در ارائه
        }

        // بدون این خط، سرور معمولاً تا ابد منتظر پیام بعدی می‌ماند
        await call.RequestStream.CompleteAsync();
        Console.WriteLine("  → CompleteAsync()");

        var result = await call;
        Console.WriteLine($"  ← خلاصه: تعداد={result.Count} | جمع={result.TotalAmount:N0}");
    }
}
