using Grpc.Net.Client;
using GrpcDemo.ConsoleClient.Demos;
using GrpcDemo.Contracts;

// Allow unencrypted HTTP/2 on localhost (demo only)
AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

const string serverAddress = "http://localhost:5051";

Console.WriteLine("========================================");
Console.WriteLine("  gRPC Console Client - code walkthrough");
Console.WriteLine("  Blazor = UI  |  Console = readable code");
Console.WriteLine("========================================");
Console.WriteLine($"Server: {serverAddress}");
Console.WriteLine();

using var channel = GrpcChannel.ForAddress(serverAddress);
var client = new AccountService.AccountServiceClient(channel);

while (true)
{
    Console.WriteLine("""
        ---- Menu ----
        1) Unary          - GetBalance (+ Headers / Deadline)
        2) Server Stream  - StreamTransactions
        3) Client Stream  - DepositBatch
        4) Bidirectional  - Chat
        0) Exit
        """);
    Console.Write("Select: ");
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
                Console.WriteLine("Invalid option.");
                break;
        }
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Error: {ex.Message}");
        Console.ResetColor();
    }

    Console.WriteLine();
}
