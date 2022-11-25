using Microsoft.AspNetCore.Mvc;
using MedBrat.Areas.Account.Models;
using MedBrat.Areas.Account.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using MedBrat.Areas.Account.Services;
using Microsoft.AspNetCore.Authorization;

namespace MedBrat.Areas.Account.Controllers
{
    public class AuthController : Controller
    {
        AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated) return RedirectToAction("Index", "Profile");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLoginViewModel userLog)
        {
            if (ModelState.IsValid)
            {
                User? user = _authService.ValidateUser(userLog).Result;
                if (user != null)
                {
                    await Authenticate(user);

                    return RedirectToAction("Index", "Profile");
                }
                ModelState.AddModelError("", "Некорректные номер полиса и(или) пароль");
            }
            return View(userLog);
        }

        [HttpGet]
        public IActionResult Registration()
        {
            if (User.Identity.IsAuthenticated) return RedirectToAction("Index", "Profile");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registration(UserRegistrationViewModel userReg)
        {
            if (userReg.Name == "admin")
                ModelState.AddModelError("Name", "admin - запрещенное имя.");

            if (ModelState.IsValid)
            {
                User? user = _authService.RegisterUser(userReg).Result;
                if (user != null)
                {
                    await Authenticate(user);

                    return RedirectToAction("Index", "Profile");
                }
                else ModelState.AddModelError("", "Данный номер полиса уже зарегистрирован");
            }

            return View(userReg);
        }

        private async Task Authenticate(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Polis),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role?.Name)
            };
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Auth");
        }
    }
}
