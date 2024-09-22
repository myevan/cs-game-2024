using MessagePack;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RpcServer.Framework
{
    public static class WebApplicationBuilderExtension
    {
        public static void InitializeRpc(this WebApplicationBuilder builder, List<IRpcHandler> handlerList)
        {
            builder.Services.AddSingleton<IRpcService>(provider => 
                new RpcService(provider.GetRequiredService<ILogger<RpcService>>(), handlerList));
        }
    }

    public static class WebApplicationExtension
    {
        public static void MapGetRpc(this WebApplication app, string pattern)
        {
            app.MapGet($"{pattern}/{{method}}", async (HttpContext httpCtx, IRpcService rpcSvc, string method) =>
            {
                var querySeq = GetSequence(httpCtx.Request.Query);
                var queryArgDict = httpCtx.Request.Query.ToDictionary(pair => pair.Key, pair => (object)pair.Value.ToString());
                var rpcReq = RpcRequest.From(querySeq, method, queryArgDict);
                var rpcRes = rpcSvc.Invoke(httpCtx, rpcReq);
                await DumpAsync(httpCtx, rpcRes);
            });
        }

        public static void MapPostRpcBatch(this WebApplication app, string pattern)
        {
            app.MapPost(pattern, async (HttpContext httpCtx, IRpcService rpcSvc) =>
            {
                var contentType = GetContentType(httpCtx.Request.Headers);
                var bodyReqList = await LoadJsonStreamAsync<List<RpcRequest>>(httpCtx.Request.Body, contentType);
                if (bodyReqList == null)
                {
                    httpCtx.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await DumpAsync(httpCtx, new List<RpcResponse>());
                    return;
                }

                var outResList = new List<RpcResponse>(bodyReqList.Count);
                foreach (var eachReq in bodyReqList)
                {
                    app.Logger.LogInformation(eachReq.Method);

                    var eachRes = rpcSvc.Invoke(httpCtx, eachReq);
                    outResList.Add(eachRes);
                }

                await DumpAsync(httpCtx, outResList);
            });

        }

        private static ulong GetSequence(IQueryCollection queryDict)
        {
            var str = queryDict["seq"].ToString();
            if (string.IsNullOrEmpty(str))
            {
                return 0;
            }

            return ulong.Parse(str);
        }

        private static string GetContentType(IHeaderDictionary headerDict)
        {
            var str = headerDict["Content-Type"].ToString();
            if (string.IsNullOrEmpty(str))
            {
                return "application/json";
            }
            return str;
        }

        private static async Task<T?> LoadJsonStreamAsync<T>(Stream stream, string contentType) where T : class
        {
            using (var reader = new StreamReader(stream))
            {
                var str = await reader.ReadToEndAsync();

                return JsonSerializer.Deserialize<T>(str);
            }
        }
        private static async Task DumpJsonAsync(HttpContext httpCtx, object objRes)
        {         
            var strRes = JsonSerializer.Serialize(objRes, JsonSerializerOpts);
            var bufRes = Encoding.UTF8.GetBytes(strRes);

            httpCtx.Response.ContentType = "application/json";
            await httpCtx.Response.Body.WriteAsync(bufRes);
        }

        private static async Task DumpMsgpLzBase64Async(HttpContext httpCtx, object objRes)
        {
            var bufRes = MessagePackSerializer.Serialize(objRes, MsgpSerializerOpts);
            var base64str = Convert.ToBase64String(bufRes);

            httpCtx.Response.ContentType = "text/plain";
            await httpCtx.Response.WriteAsync(base64str);
        }

        private static async Task DumpMsgpBase64Async(HttpContext httpCtx, object objRes)
        {
            var bufRes = MessagePackSerializer.Serialize(objRes);
            var base64str = Convert.ToBase64String(bufRes);
            
            httpCtx.Response.ContentType = "text/plain";
            await httpCtx.Response.WriteAsync(base64str);
        }

        private static async Task DumpMsgpAsync(HttpContext httpCtx, object objRes)
        {
            var bufRes = MessagePackSerializer.Serialize(objRes, MsgpSerializerOpts);
            var base64str = Convert.ToBase64String(bufRes);
            
            httpCtx.Response.ContentType = "application/x-msgpack";
            await httpCtx.Response.Body.WriteAsync(bufRes);
        }

        private static async Task DumpAsync(HttpContext httpCtx, object objRes)
        {
            var outEncoding = httpCtx.Request.Query["out"].ToString();
            switch (outEncoding)
            {
                case "json":
                    await DumpJsonAsync(httpCtx, objRes);
                    break;
                case "msgp-lz-base64":
                    await DumpMsgpLzBase64Async(httpCtx, objRes);
                    break;
                case "msgp-base64":
                    await DumpMsgpBase64Async(httpCtx, objRes);
                    break;
                default:
                    await DumpJsonAsync(httpCtx, objRes);
                    break;
            }
        }

        private readonly static JsonSerializerOptions JsonSerializerOpts = new JsonSerializerOptions
        {
            PropertyNamingPolicy = null, // PascalCase 유지
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, // null 필드 제외
        };

        private readonly static MessagePackSerializerOptions MsgpSerializerOpts =
            MessagePackSerializerOptions.Standard
                .WithCompressionMinLength(1024) // 저용량 압축 효율 낮음
                .WithCompression(MessagePackCompression.Lz4BlockArray);
                
    }

}
