using Dapper;
using Microsoft.Extensions.Options;
using RpcServer.Framework;
using System.Data;

namespace RpcServer.Application
{
    public class Repository : IDisposable
    {
        public Repository(DbConnectionFactory dbFactory, string dbConnKey)
        {
            _isDisposed = false;

            _dbConn = dbFactory.CreateDbConnection(dbConnKey);

            EnsureOpen();

            _dbTransaction = _dbConn.BeginTransaction();
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;

            _dbTransaction.Dispose();

            _dbConn.Dispose();
        }

        public void Commit()
        {
            _dbTransaction.Commit();
        }

        public void Rollback()
        {
            _dbTransaction.Rollback();
        }

        protected IDbConnection EnsureOpen()
        {
            if (_dbConn.State == ConnectionState.Closed)
            {
                _dbConn.Open();
            }

            return _dbConn;
        }

        private bool _isDisposed = false;

        private readonly IDbConnection _dbConn;
        private readonly IDbTransaction _dbTransaction;
    }

    public class AuthRepository : Repository
    {
        public AuthRepository(DbConnectionFactory dbFactory, IOptions<AppSettings.ConnectionKeys> dbConnKeys) : 
            base(dbFactory, dbConnKeys.Value.Auth)
        {          
        }

        public AccountDevice? GetDevice(string inIdfv)
        {
            var dbConn = EnsureOpen();
            return dbConn.Query<AccountDevice>("SELECT * FROM AccountDevice WHERE Idfv = @Idfv", new { Idfv = inIdfv }).FirstOrDefault();
        }

        public Account? GetAccount(ulong inId)
        {
            var dbConn = EnsureOpen();
            return dbConn.Query<Account>("SELECT * FROM Account WHERE Id = @Id", new { Id = inId }).FirstOrDefault();
        }

        public void Save(Account inAccount)
        {
            var dbConn = EnsureOpen();
            var retDeviceId = dbConn.ExecuteScalar<ulong>(
                "INSERT INTO Account (Id, ClientRandSeed, ServerRandSeed) VALUES(@Id, @ClientRandSeed, @ServerRandSeed)", inAccount);
        }

        public void Save(AccountDevice inDevice)
        {
            var dbConn = EnsureOpen();
            dbConn.ExecuteScalar<ulong>(
                "INSERT INTO AccountDevice (Idfv, AccountId) VALUES(@Idfv, @AccountId)", inDevice);                     
        }
    }
}