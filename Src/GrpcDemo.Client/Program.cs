using GrpcDemo.Contracts;

AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddGrpcClient<AccountService.AccountServiceClient>(options =>
{
    options.Address = new Uri(builder.Configuration["GrpcServer"] ?? "http://localhost:5051");
});

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
