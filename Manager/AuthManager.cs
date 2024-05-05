using System.Security.Claims;
using Dapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using SimpleAuth.Entity;
using SimpleAuth.Manager.Interfaces;
using SimpleAuth.Provider.Interfaces;

namespace SimpleAuth.Manager;

public class AuthManager : IAuthManager
{
    private readonly IDbConnectionProvider connectionProvider;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthManager(IDbConnectionProvider connectionProvider, IHttpContextAccessor httpContextAccessor)
    {
        this.connectionProvider = connectionProvider;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task Login(string username, string password)
    {
        var user = await GetUserByUsername(username);
        if (user == null)
        {
            throw new Exception("Invalid username");
        }

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            throw new Exception("Username and password do not match");
        }

        var httpContext = _httpContextAccessor.HttpContext;
        var claims = new List<Claim>
        {
            new("Id", user.Id.ToString())
        };
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity));
    }

    private async Task<User> GetUserByUsername(string username)
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
from auth_user WHERE email = @username
";
        return await conn.QueryFirstOrDefaultAsync<User>(query, new
        {
            username = username
        });
    }

    public async Task Logout()
    {
        await _httpContextAccessor.HttpContext.SignOutAsync();
    }
}