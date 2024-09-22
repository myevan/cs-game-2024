using RpcServer.Benchmark;
using RpcServer.Framework;

var builder = WebApplication.CreateSlimBuilder(args);
builder.InitializeRpc(new List<IRpcHandler>
{
    new RpcHandler<BenchmarkService>("benchmark-hello", (svc, req) => svc.Hello(req)),
    new RpcHandler<BenchmarkService>("benchmark-large", (svc, req) => svc.Large(req)),
});
builder.Services.AddScoped<BenchmarkService>();

var app = builder.Build();
app.MapGet("/", () =>
{
    return "Hello World!";
});
app.MapGetRpc("/rpc");
app.MapPostRpcBatch("/rpc-batch");

app.Run();