using Npgsql;
using SimpleAuth.Provider.Interfaces;
using System.Data;

namespace SimpleAuth.Provider
{
    public class DbConnectionProvider(IConfiguration configuration) : IDbConnectionProvider
    {
        public IDbConnection GetDbConnection() => new NpgsqlConnection(configuration["ConnectionString:Default"]);
    }
}
