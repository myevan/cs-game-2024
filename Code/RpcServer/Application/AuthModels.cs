namespace RpcServer.Application
{
    public class Account
    {
        public long Id { get; set; } = 0;
        public int ClientRandSeed { get; set; } = 0;
        public int ServerRandSeed { get; set; } = 0;

        public DateTime CreateTime { get; set; } = DateTime.Now;
        public DateTime UpdateTime { get; set; } = DateTime.Now;
    }

    public class AccountDevice
    {
        public long AccountId { get; set; } = 0;
        public string Idfv { get; set; } = string.Empty;

        public DateTime CreateTime { get; set; } = DateTime.Now;
        public DateTime UpdateTime { get; set; } = DateTime.Now;
    }

    public class AccountSession
    {
        public long AccountId { get; set; } = 0;
        public string Id {  get; set; } = string.Empty;
        public string Payload { get; set; } = string.Empty;

        public DateTime CreateTime { get; set; } = DateTime.Now;
        public DateTime UpdateTime { get; set; } = DateTime.Now;
    }
}
