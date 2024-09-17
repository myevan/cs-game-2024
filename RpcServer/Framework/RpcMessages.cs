namespace RpcServer.Framework
{
    public class RpcRequest
    {

        public static RpcRequest From(ulong inSeq, string inMethod, Dictionary<string, object> inArgDict)
        {
            var outReq = new RpcRequest();
            outReq.Seq = inSeq;
            outReq.Method = inMethod;
            outReq.ArgDict = inArgDict;
            return outReq;
        }

        public T MapArgDict<T>() where T: class, new()
        {
            return RpcHelper.Map<T>(ArgDict);
        }

        public ulong Seq { get; set; } = 0; // Sequence


        public string Method { get; set; } = string.Empty;


        public Dictionary<string, object> ArgDict { get; set; } = new();
    }

    public class RpcError
    {
        public static RpcError From(string inMsg)
        {
            var outErr = new RpcError();
            outErr.Msg = inMsg;
            return outErr;
        }

        public ulong Code { get; private set; } = 0;

        public string Msg { get; private set; } = string.Empty;

        public Dictionary<string, object> CtxDict { get; private set; } = new();

    }

    public class RpcResponse
    {
        public static RpcResponse From(RpcRequest inReq)
        {
            var outRes = new RpcResponse();
            outRes.Seq = inReq.Seq;
            outRes.Method = inReq.Method;
            return outRes;
        }

        public RpcResponse BindError(RpcError inError)
        {
            Error = inError;
            return this;
        }

        public RpcResponse AppendNewString(string inKey, string inStr)
        {
            var ctxStrList = RpcHelper.TouchCollection<List<string>>(ref _newDict, inKey);
            ctxStrList.Add(inStr);
            return this;
        }

        public ulong Seq { get; set; } = 0;

        public string Method {  get; set; } = string.Empty;

        public RpcError? Error { get; set; } = null;

        public Dictionary<string, object> NewDict { get { return _newDict; } }

        private Dictionary<string, object> _newDict = new();
    }


}
