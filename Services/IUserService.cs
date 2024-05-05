using SimpleAuth.Entity;

namespace SimpleAuth.Services
{
    public interface IUserService
    {
        Task CreateUser(User user, string password);
    }
}