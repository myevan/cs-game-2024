namespace RpcServer.Game.Data
{
    public class SchemeConfig
    {
        public class FieldDecl
        {
            public string Name { get; set; } = string.Empty;
            public string Type { get; set; } = string.Empty;
        }

        public string Name { get; set; } = string.Empty;

        public List<FieldDecl> FieldDeclList { get; set; } = new();
    }

    public class ProtoConfig
    {
        public List<SchemeConfig> SchemeConfigList { get; set; } = new();
    }
}
