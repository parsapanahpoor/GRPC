using Grpc.Core;
using GrpcDemo.Contracts;

namespace GrpcDemo.Server.Services;

public class AccountGrpcService : AccountService.AccountServiceBase
{
    // تا در دمو استریم به‌وضوح دیده شود
    private const int StreamDelayMs = 1500;
    private const int ChatReplyDelayMs = 1200;

    private static readonly Dictionary<string, double> Balances = new()
    {
        ["1001"] = 15_000_000,
        ["1002"] = 8_500_000,
        ["1003"] = 2_200_000
    };

    public override async Task<GetBalanceResponse> GetBalance(
        GetBalanceRequest request,
        ServerCallContext context)
    {
        var correlationId = context.RequestHeaders.GetValue("x-correlation-id");
        Console.WriteLine($"GetBalance | correlation-id={correlationId ?? "n/a"}");

        var delayHeader = context.RequestHeaders.GetValue("x-demo-delay-ms");
        if (int.TryParse(delayHeader, out var delayMs) && delayMs > 0)
        {
            Console.WriteLine($"GetBalance | demo delay {delayMs}ms");
            await Task.Delay(delayMs, context.CancellationToken);
        }

        if (!Balances.TryGetValue(request.AccountNumber, out var balance))
        {
            throw new RpcException(new Status(
                StatusCode.NotFound,
                $"Account {request.AccountNumber} was not found."));
        }

        return new GetBalanceResponse
        {
            AccountNumber = request.AccountNumber,
            Balance = balance,
            Currency = "IRR"
        };
    }

    // لیست تراکنش‌های یک حساب به‌صورت تدریجی 
    public override async Task StreamTransactions(
        StreamTransactionsRequest request,
        IServerStreamWriter<Transaction> responseStream,
        ServerCallContext context)
    {
        var limit = request.Limit <= 0 ? 5 : Math.Min(request.Limit, 8);

        for (var i = 1; i <= limit; i++)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            await Task.Delay(StreamDelayMs, context.CancellationToken);

            await responseStream.WriteAsync(new Transaction
            {
                Id = Guid.NewGuid().ToString("N")[..8],
                AccountNumber = request.AccountNumber,
                Amount = 100_000 * i,
                Type = i % 2 == 0 ? "Credit" : "Debit",
                Description = $"Sample transaction #{i} of {limit}"
            });
        }
    }

    public override async Task<DepositBatchResponse> DepositBatch(
        IAsyncStreamReader<DepositRequest> requestStream,
        ServerCallContext context)
    {
        var count = 0;
        var total = 0.0;

        await foreach (var deposit in requestStream.ReadAllAsync(context.CancellationToken))
        {
            await Task.Delay(StreamDelayMs, context.CancellationToken);

            if (!Balances.ContainsKey(deposit.AccountNumber))
                Balances[deposit.AccountNumber] = 0;

            Balances[deposit.AccountNumber] += deposit.Amount;
            count++;
            total += deposit.Amount;
            Console.WriteLine($"DepositBatch | received #{count}: {deposit.Amount:N0}");
        }

        return new DepositBatchResponse
        {
            Count = count,
            TotalAmount = total
        };
    }

    public override async Task Chat(
        IAsyncStreamReader<ChatMessage> requestStream,
        IServerStreamWriter<ChatMessage> responseStream,
        ServerCallContext context)
    {
        await responseStream.WriteAsync(new ChatMessage
        {
            User = "Support",
            Text = "Welcome to the gRPC chat room. Send a message; I am here.",
            SentAt = DateTime.Now.ToString("HH:mm:ss")
        });

        await foreach (var incoming in requestStream.ReadAllAsync(context.CancellationToken))
        {
            Console.WriteLine($"Chat | {incoming.User}: {incoming.Text}");

            await Task.Delay(ChatReplyDelayMs, context.CancellationToken);

            await responseStream.WriteAsync(new ChatMessage
            {
                User = "Support",
                Text = $"Received: \"{incoming.Text}\" — reply on the same bidirectional stream.",
                SentAt = DateTime.Now.ToString("HH:mm:ss")
            });
        }
    }
}
