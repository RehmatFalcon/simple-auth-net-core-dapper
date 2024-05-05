using System.Security.Claims;
using Dapper;
using SimpleAuth.Entity;
using SimpleAuth.Provider.Interfaces;

namespace SimpleAuth.Provider;

public class CurrentUserProvider : ICurrentUserProvider
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IDbConnectionProvider connectionProvider;

    public CurrentUserProvider(IHttpContextAccessor contextAccessor, IDbConnectionProvider connectionProvider)
    {
        _contextAccessor = contextAccessor;
        this.connectionProvider = connectionProvider;
    }

    public bool IsLoggedIn()
        => GetCurrentUserId() != null;

    public async Task<User?> GetCurrentUser()
    {
        var currentUserId = GetCurrentUserId();
        if (!currentUserId.HasValue) return null;

        return await GetUserById(currentUserId.Value);
    }

    private async Task<User?> GetUserById(long userId)
    {
        using var conn = connectionProvider.GetDbConnection();

        var query = @"
select  id as Id
,name as Name
,email as Email
,password_hash as PasswordHash
,user_status as UserStatus
,user_type as UserType
,created_date as CreatedDate
from auth_user WHERE id = @id
";
        return await conn.QueryFirstOrDefaultAsync<User>(query, new
        {
            id = userId
        });
    }

    public long? GetCurrentUserId()
    {
        var userId = _contextAccessor.HttpContext?.User.FindFirstValue("Id");
        if (string.IsNullOrWhiteSpace(userId)) return null;
        return Convert.ToInt64(userId);
    }
}