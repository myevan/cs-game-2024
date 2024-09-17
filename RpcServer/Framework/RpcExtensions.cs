using Microsoft.AspNetCore.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RpcServer.Framework
{
    public static class WebApplicationBuilderExtension
    {
        public static void InitializeRpc(this WebApplicationBuilder builder, List<IRpcHandler> handlerList)
        {
            builder.Services.Configure<JsonOptions>(options =>
            {
                options.SerializerOptions.PropertyNamingPolicy = null; // PascalCase 유지
                options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull; // null 필드 제외
            });

            builder.Services.AddSingleton<IRpcService>(provider => 
                new RpcService(provider.GetRequiredService<ILogger<RpcService>>(), handlerList));
        }
    }

    public static class WebApplicationExtension
    {
        public static void MapGetRpc(this WebApplication app, string pattern)
        {
            app.MapGet($"{pattern}/{{method}}", (string method, HttpContext httpCtx, IRpcService rpcSvc) =>
            {
                var querySeq = GetSequence(httpCtx.Request.Query);
                var queryArgDict = httpCtx.Request.Query.ToDictionary(pair => pair.Key, pair => (object)pair.Value.ToString());
                var rpcReq = RpcRequest.From(querySeq, method, queryArgDict);
                var rpcRes = rpcSvc.Invoke(httpCtx, rpcReq);
                return Results.Ok(rpcRes);
            });
        }

        public static void MapPostRpcBatch(this WebApplication app, string pattern)
        {
            app.MapPost(pattern, async (HttpContext httpCtx, IRpcService rpcSvc) =>
            {
                var contentType = GetContentType(httpCtx.Request.Headers);
                var bodyReqList = await MapFromStreamAsync<List<RpcRequest>>(httpCtx.Request.Body, contentType);
                if (bodyReqList == null)
                {
                    return Results.BadRequest();
                }

                var outResList = new List<RpcResponse>(bodyReqList.Count);
                foreach (var eachReq in bodyReqList)
                {
                    app.Logger.LogInformation(eachReq.Method);

                    var eachRes = rpcSvc.Invoke(httpCtx, eachReq);
                    outResList.Add(eachRes);
                }

                return Results.Ok(outResList);
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

        private static async Task<T?> MapFromStreamAsync<T>(Stream stream, string contentType) where T : class
        {
            using (var reader = new StreamReader(stream))
            {
                var str = await reader.ReadToEndAsync();

                return JsonSerializer.Deserialize<T>(str);
            }
        }
    }

}
