using System.Transactions;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleAuth.Constants;
using SimpleAuth.Entity;
using SimpleAuth.Provider.Interfaces;
using SimpleAuth.Services;

namespace SimpleAuth.Controllers;

[AllowAnonymous]
public class SeedingController : Controller
{
    private readonly IDbConnectionProvider connectionProvider;
    private readonly IUserService userService;

    public SeedingController(IDbConnectionProvider connectionProvider, IUserService userService)
    {
        this.connectionProvider = connectionProvider;
        this.userService = userService;
    }

    public async Task<IActionResult> SeedSuperAdmin()
    {
        try
        {
            var previousSuperAdminExists = await DoesPreviousSuperAdminExists();
            if (!previousSuperAdminExists)
            {
                using var tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
                var admin = new User()
                {
                    Email = "super.admin",
                    UserType = UserTypeConstants.Admin,
                    Name = "Super Admin"
                };
                await userService.CreateUser(admin, "admin");
                tx.Complete();
                return Content("User Seeding Complete");
            }

            return Content("User already seeded");
        }
        catch (Exception e)
        {
            return Content(e.Message);
        }
    }

    private async Task<bool> DoesPreviousSuperAdminExists()
    {
        using var conn = connectionProvider.GetDbConnection();
        var query = @"
select count(*) from auth_user WHERE user_type = @user_type
";
        return (await conn.QueryFirstOrDefaultAsync<int>(query, new { user_type = UserTypeConstants.Admin })) > 0;
    }
}