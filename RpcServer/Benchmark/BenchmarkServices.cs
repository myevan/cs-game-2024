using RpcServer.Framework;

namespace RpcServer.Benchmark
{
    public class BenchmarkService
    {
        public RpcResponse Hello(RpcRequest req)
        {
            var reqHello = req.MapArgDict<BenchmarkHello>();
            return RpcResponse.From(req).AppendNewString("MsgList", $"Hello, {reqHello.Name}!");
        }
    }
}
