using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleAuth.Manager.Interfaces;
using SimpleAuth.Provider.Interfaces;
using SimpleAuth.ViewModels.Auth;

namespace SimpleAuth.Controllers;

[AllowAnonymous]
public class AuthController : Controller
{
    private readonly IDbConnectionProvider connectionProvider;
    private readonly IAuthManager _authManager;

    public AuthController(IDbConnectionProvider connectionProvider, IAuthManager authManager)
    {
        this.connectionProvider = connectionProvider;
        _authManager = authManager;
    }

    public IActionResult Login()
    {
        var vm = new LoginVm();
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginVm vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        try
        {
            await _authManager.Login(vm.Username, vm.Password);
            return RedirectToAction("Index", "Home");
        }
        catch (Exception e)
        {
            vm.ErrorMessage = e.Message;
            return View(vm);
        }
    }

    public async Task<IActionResult> Logout()
    {
        await _authManager.Logout();
        return RedirectToAction("Login");
    }
}