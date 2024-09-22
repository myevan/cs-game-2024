using System.Collections.Concurrent;

namespace RpcServer.Framework
{
    public class RpcService : IRpcService
    {
        public RpcService(ILogger<RpcService> logger, IEnumerable<IRpcHandler> handlerList)
        {
            _logger = logger;

            _handlerList = handlerList;

            foreach (var eachHandler in _handlerList)
            {
                _logger.LogDebug($"* rpc_method({eachHandler.Method})");
                if (!_declDict.TryAdd(eachHandler.Method, eachHandler))
                {
                    _logger.LogError($"NOT_ADDED_METHOD({eachHandler.Method})");
                    continue;
                }
            }
        }

        public RpcResponse Invoke(HttpContext httpCtx, RpcRequest req)
        {
            if (!_declDict.TryGetValue(req.Method, out var rpcDecl))
            {
                return RpcResponse.From(req).BindError(RpcError.From($"NOT_FOUND_METHOD({req.Method})"));
            }

            return rpcDecl.Invoke(httpCtx, req);
        }


        public IEnumerable<IRpcHandler> HandlerlList { get { return _handlerList; } }

        private readonly IEnumerable<IRpcHandler> _handlerList;
        
        private readonly ILogger<RpcService> _logger;

        private readonly ConcurrentDictionary<string, IRpcHandler> _declDict = new();

    }
}
