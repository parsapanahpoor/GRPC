using Grpc.Net.Client;
using GrpcDemo.ConsoleClient.Demos;
using GrpcDemo.Contracts;

// برای HTTP بدون TLS روی localhost (دمو)
AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

Console.OutputEncoding = System.Text.Encoding.UTF8;

const string serverAddress = "http://localhost:5051";

Console.WriteLine("========================================");
Console.WriteLine("  gRPC Console Client — برای توضیح کد");
Console.WriteLine("  Blazor = UI  |  این پروژه = کد خوانا");
Console.WriteLine("========================================");
Console.WriteLine($"سرور: {serverAddress}");
Console.WriteLine();

using var channel = GrpcChannel.ForAddress(serverAddress);
var client = new AccountService.AccountServiceClient(channel);

while (true)
{
    Console.WriteLine("""
        ---- منو ----
        1) Unary          — GetBalance (+ Headers / Deadline)
        2) Server Stream  — StreamTransactions
        3) Client Stream  — DepositBatch
        4) Bidirectional  — Chat
        0) خروج
        """);
    Console.Write("انتخاب: ");
    var choice = Console.ReadLine()?.Trim();

    try
    {
        switch (choice)
        {
            case "1":
                await UnaryDemo.RunAsync(client);
                break;
            case "2":
                await ServerStreamingDemo.RunAsync(client);
                break;
            case "3":
                await ClientStreamingDemo.RunAsync(client);
                break;
            case "4":
                await ChatDemo.RunAsync(client);
                break;
            case "0":
                return;
            default:
                Console.WriteLine("گزینه نامعتبر.");
                break;
        }
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"خطا: {ex.Message}");
        Console.ResetColor();
    }

    Console.WriteLine();
}
