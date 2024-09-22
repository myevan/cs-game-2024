using MessagePack;

namespace RpcServer.Framework
{
    [MessagePackObject]
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

        [Key(0)]
        public ulong Seq { get; set; } = 0; // Sequence

        [Key(1)]
        public string Method { get; set; } = string.Empty;

        [Key(2)]
        public Dictionary<string, object> ArgDict { get; set; } = new();
    }

    [MessagePackObject]
    public partial class RpcError
    {
        public static RpcError From(string inMsg)
        {
            var outErr = new RpcError();
            outErr.Msg = inMsg;
            return outErr;
        }

        [Key(0)]
        public ulong Code { get; private set; } = 0;

        [Key(1)]
        public string Msg { get; private set; } = string.Empty;

        [Key(2)]
        public Dictionary<string, object> CtxDict { get; private set; } = new();

    }

    [MessagePackObject]
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

        [Key(0)]
        public ulong Seq { get; set; } = 0;

        [Key(1)]
        public string Method {  get; set; } = string.Empty;

        [Key(2)]
        public RpcError? Error { get; set; } = null;

        [Key(3)]
        public Dictionary<string, object> NewDict { get { return _newDict; } set { _newDict = value; } }

        [IgnoreMember]
        private Dictionary<string, object> _newDict = new();
    }


}
