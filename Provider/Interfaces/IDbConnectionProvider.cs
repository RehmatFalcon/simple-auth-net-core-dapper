using System.Data;

namespace SimpleAuth.Provider.Interfaces
{
    public interface IDbConnectionProvider
    {
        IDbConnection GetDbConnection();
    }
}