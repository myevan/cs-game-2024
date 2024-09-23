using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Hosting.Server;
using RpcServer.Framework;

namespace RpcServer.Application
{
    public class SnowflakeApiService
    {
        public SnowflakeApiService(SnowflakeFactory factory, HttpClient httpClient, IServer server)
        {
            _factory = factory;
            _httpClient = httpClient;
            _server = server;
        }

        public long CreateAccountId()
        {
            return _factory.CreateId(0);
        }

        public async Task<long> RequestAccountIdAsync()
        {
            var idGenServerUrl = GetIdGenServerUrl();
            var idGenApiUrl = $"{idGenServerUrl}/snowflakg/0?out=plain";
            var resId = await _httpClient.GetAsync(idGenApiUrl);
            resId.EnsureSuccessStatusCode();

            var newAccountIdStr = await resId.Content.ReadAsStringAsync();
            return long.Parse(newAccountIdStr);
        }

        private string GetIdGenServerUrl()
        {
            var svrAddrFeature = _server.Features.Get<IServerAddressesFeature>();
            if (svrAddrFeature != null)
            {
                foreach (var eachAddr in svrAddrFeature.Addresses)
                {
                    return eachAddr;
                }
            }

            return "http://localhost:5000";
        }

        private readonly SnowflakeFactory _factory;
        private HttpClient _httpClient;
        private IServer _server;
    }
}
