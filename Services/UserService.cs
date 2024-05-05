using BCrypt.Net;
using Dapper;
using SimpleAuth.Constants;
using SimpleAuth.Entity;
using SimpleAuth.Provider.Interfaces;

namespace SimpleAuth.Services
{
    public class UserService : IUserService
    {
        private readonly IDbConnectionProvider connectionProvider;

        public UserService(IDbConnectionProvider connectionProvider)
        {
            this.connectionProvider = connectionProvider;
        }

        public async Task CreateUser(User user, string password)
        {
            using var conn = connectionProvider.GetDbConnection();

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var query = @"
INSERT INTO auth_user (name, email, password_hash, user_status, user_type, created_date)
values (@name, @email, @password_hash, @user_status, @user_type, @created_date)
";
            await conn.ExecuteAsync(query, new
            {
                name = user.Name,
                email = user.Email,
                password_hash = passwordHash,
                user_status = UserStatusConstants.Active,
                user_type = user.UserType,
                created_date = DateTime.UtcNow
            });
        }
    }
}
