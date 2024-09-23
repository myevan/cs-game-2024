using RpcServer.Application;
using RpcServer.Benchmark;
using RpcServer.Framework;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddDebug(); // Debug Output 창에 로그를 표시
builder.Logging.AddConsole(); // 콘솔에 로그를 표시 (선택 사항)

builder.Services.Configure<AppSettings.ConnectionKeys>(builder.Configuration.GetSection("ConnectionKeys"));

builder.Services.AddHttpClient<SnowflakeApiService>((client) => client.Timeout = TimeSpan.FromSeconds(30));

builder.Services.AddSingleton<SnowflakeFactory>();
builder.Services.AddSingleton<DbConnectionFactory>();

builder.Services.AddScoped<SnowflakeApiService>();
builder.Services.AddScoped<AuthRepository>();
builder.Services.AddScoped<DeviceAuthService>();

builder.InitializeRpc(new List<IRpcHandler>
{
    new RpcHandler<BenchmarkService>("benchmark-hello", (svc, req) => svc.Hello(req)),
    new RpcHandler<BenchmarkService>("benchmark-large", (svc, req) => svc.Large(req)),
});
builder.Services.AddScoped<BenchmarkService>();

var app = builder.Build();
app.MapGet("/", async (DeviceAuthService authSvc) =>
{
    await authSvc.ConnectAsync("AAA");
    return "Hello World!";
});

app.MapGet("/api/snowflake/account-id", (SnowflakeApiService svc) => svc.CreateAccountId());
app.MapGet("/api/device-auth/connect/{idfv}", async (DeviceAuthService svc, string idfv) => await svc.ConnectAsync(idfv));

app.MapGetRpc("/rpc");
app.MapPostRpcBatch("/rpc-batch");

app.MapGet("/api/auth", (AuthRepository authRepo) =>
{
    return "Hello Auth!";
});

app.Run();