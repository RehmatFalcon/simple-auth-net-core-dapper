using SimpleAuth.Entity;

namespace SimpleAuth.Provider.Interfaces
{
    public interface ICurrentUserProvider
    {
        Task<User?> GetCurrentUser();
        long? GetCurrentUserId();
        bool IsLoggedIn();
    }
}