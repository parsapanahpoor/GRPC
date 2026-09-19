using Grpc.Core;
using GrpcDemo.Contracts;

namespace GrpcDemo.ConsoleClient.Demos;

/// <summary>
/// Bidirectional Streaming: چت‌روم
/// کانال باز می‌ماند؛ کلاینت و سرور هم‌زمان پیام رد و بدل می‌کنند
/// </summary>
public static class ChatDemo
{
    public static async Task RunAsync(AccountService.AccountServiceClient client)
    {
        Console.WriteLine();
        Console.WriteLine("=== Bidirectional: Chat ===");
        Console.WriteLine("چند پیام می‌فرستیم؛ پشتیبان روی همان استریم جواب می‌دهد.");
        Console.WriteLine("برای پایان، خالی Enter بزنید.");
        Console.WriteLine();

        using var call = client.Chat();

        // حلقهٔ خواندن پاسخ‌های سرور (هم‌زمان با نوشتن)
        var readTask = Task.Run(async () =>
        {
            await foreach (var msg in call.ResponseStream.ReadAllAsync())
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  ← {msg.User} ({msg.SentAt}): {msg.Text}");
                Console.ResetColor();
            }
        });

        // چند پیام نمونه برای ارائه (بدون تایپ دستی)
        string[] demoMessages =
        [
            "سلام، موجودی حسابم چند است؟",
            "ممنون. پس gRPC دوطرفه یعنی همین کانال باز؟"
        ];

        foreach (var text in demoMessages)
        {
            var now = DateTime.Now.ToString("HH:mm:ss");
            await call.RequestStream.WriteAsync(new ChatMessage
            {
                User = "ارائه‌دهنده",
                Text = text,
                SentAt = now
            });

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"  → من ({now}): {text}");
            Console.ResetColor();

            await Task.Delay(2000); // فرصت دیدن جواب پشتیبان
        }

        await call.RequestStream.CompleteAsync();
        await readTask;

        Console.WriteLine("چت بسته شد.");
    }
}
