namespace RpcServer.Game.Data
{
    public class GameProtoAsset
    {
        public List<HeroProto> HeroProtoList { get; set; } = new();

        public Dictionary<int, int> HeroProtoNumToRow { get; set; } = new();

        public Dictionary<string, int> HeroProtoDivToRow { get; set; } = new();
    }
}
