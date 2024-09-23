using IdGen;
using System.Data;
using System.Data.SQLite;

namespace RpcServer.Framework
{
    public class SnowflakeFactory
    {
        public long CreateId(int workerId)
        {
            if (workerId > _idGenList.Count)
            {
                return 0;
            }

            var selectedIdGen = _idGenList[workerId];
            return selectedIdGen.CreateId();
        }

        private List<IdGenerator> _idGenList = new()
        {
            new(0),
            new(1),
            new(2),
            new(3),
        };
    }

    public class DbConnectionFactory
    {
        public DbConnectionFactory(ILogger<DbConnectionFactory> logger, IConfiguration cfg)
        {
            _logger = logger;
            _cfg = cfg;
        }

        public IDbConnection CreateDbConnection(string key)
        {   
            var connStr = _cfg.GetConnectionString(key);
            if (connStr == null)
            {
                throw new ArgumentException($"NOT_FOUND_DB_CONNECTION_KEY({key}");
            }

            if (key.StartsWith("SQLite+"))
            {
                return CreateSQLiteConnection(connStr);
            }

            throw new ArgumentException($"NOT_SUPPORTED_DB_CONNECTION_STRING_FOR_KEY({key}");
        }

        private SQLiteConnection CreateSQLiteConnection(string connStr)
        {
            return new SQLiteConnection(connStr);
        }

        private readonly ILogger _logger;
        private readonly IConfiguration _cfg;
    }
}
