namespace RpcServer.Framework
{
    public class RpcHandler<T> : IRpcHandler
    {
        public delegate RpcResponse OnInvokeDelegate(T svc, RpcRequest req);

        public RpcHandler(string inMethod, OnInvokeDelegate inOnInvoke)
        {
            Method = inMethod;
            OnInvoke = inOnInvoke;
        }

        public RpcResponse Invoke(HttpContext httpCtx, RpcRequest req)
        {
            var svc = httpCtx.RequestServices.GetService<T>();
            if (svc == null)
            {
                return RpcResponse.From(req).BindError(RpcError.From($"NOT_FOUND_SERVICE({typeof(T).Name})"));
            }

            return OnInvoke(svc, req);
        }

        public string Method { get; }

        public OnInvokeDelegate OnInvoke { get; }

        public override string ToString()
        {
            return $"RpcHandler({Method})";
        }
    }

}
