namespace RpcServer.Framework
{
    public interface IRpcHandler
    {
        string Method { get; }

        RpcResponse Invoke(HttpContext httpCtx, RpcRequest req);
    }

    public interface IRpcService
    {
        RpcResponse Invoke(HttpContext httpCtx, RpcRequest req);

        IEnumerable<IRpcHandler> HandlerlList { get; }
    }
}
