using Grpc.Core;
using GrpcDemo.Contracts;

namespace GrpcDemo.ConsoleClient.Demos;

/// <summary>
/// Bidirectional Streaming: chat room
/// Channel stays open; client and server exchange messages concurrently
/// </summary>
public static class ChatDemo
{
    public static async Task RunAsync(AccountService.AccountServiceClient client)
    {
        Console.WriteLine();
        Console.WriteLine("=== Bidirectional: Chat ===");
        Console.WriteLine("Sending messages; support replies on the same stream.");
        Console.WriteLine();

        using var call = client.Chat();

        var readTask = Task.Run(async () =>
        {
            await foreach (var msg in call.ResponseStream.ReadAllAsync())
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  <- {msg.User} ({msg.SentAt}): {msg.Text}");
                Console.ResetColor();
            }
        });

        string[] demoMessages =
        [
            "Hi, what is my account balance?",
            "Thanks. So bidirectional gRPC means this open channel?"
        ];

        foreach (var text in demoMessages)
        {
            var now = DateTime.Now.ToString("HH:mm:ss");
            await call.RequestStream.WriteAsync(new ChatMessage
            {
                User = "Presenter",
                Text = text,
                SentAt = now
            });

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"  -> Me ({now}): {text}");
            Console.ResetColor();

            await Task.Delay(2000);
        }

        await call.RequestStream.CompleteAsync();
        await readTask;

        Console.WriteLine("Chat closed.");
    }
}
