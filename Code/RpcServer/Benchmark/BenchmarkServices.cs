﻿using RpcServer.Framework;

namespace RpcServer.Benchmark
{
    public class BenchmarkService
    {
        public RpcResponse Hello(RpcRequest req)
        {
            var reqHello = req.MapArgDict<BenchmarkHello>();
            return RpcResponse.From(req).AddString("MsgList", $"Hello, {reqHello.Name}!");
        }
        public RpcResponse Large(RpcRequest req)
        {
            var reqLarge = req.MapArgDict<BenchmarkLarge>();
            var resLarge = RpcResponse.From(req);

            for (int i = 0; i != reqLarge.Count; ++i)
            {
                resLarge.AddString("MsgList", $"Item({i})");
            }
            return resLarge;
        }
    }
}
