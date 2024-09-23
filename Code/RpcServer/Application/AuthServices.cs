namespace RpcServer.Application
{
    public class DeviceAuthService
    {
        public DeviceAuthService(AuthRepository authRepo, SnowflakeApiService snowflakeApiSvc)
        {
            _authRepo = authRepo;
            _snowflakeApiSvc = snowflakeApiSvc;
        }

        public async Task ConnectAsync(string inIdfv)
        {
            var foundDevice = _authRepo.GetDevice(inIdfv);
            if (foundDevice == null)
            {
                var rand = Random.Shared;

                var newAccountId = await _snowflakeApiSvc.RequestAccountIdAsync();
                var newAccount = new Account()
                {
                    Id = newAccountId,
                    ClientRandSeed = rand.Next(),
                    ServerRandSeed = rand.Next(),
                };
                _authRepo.Save(newAccount);

                var newDevice = new AccountDevice()
                {
                    Idfv = inIdfv,
                    AccountId = newAccount.Id
                };
                _authRepo.Save(newDevice);

                _authRepo.Commit();
            }
        }

        private AuthRepository _authRepo;
        private SnowflakeApiService _snowflakeApiSvc;

    }
}
