using System;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MvcPustok.Areas.Manage.ViewModels;
using MvcPustok.Models;

namespace MvcPustok.Areas.Manage.Controllers
{
    [Area("manage")]
	public class AccountController:Controller
	{
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager,RoleManager<IdentityRole> roleManager)
		{
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> CreateAdmin()
        {
            AppUser appUser = new AppUser()
            {
                UserName = "admin"
            };
            var result = await _userManager.CreateAsync(appUser, "Admin123");
            await _userManager.AddToRoleAsync(appUser, "superadmin");

            return Json(result);
        }
        public async Task<IActionResult> CreateRoles()
        {
            await _roleManager.CreateAsync(new IdentityRole("superadmin"));
            await _roleManager.CreateAsync(new IdentityRole("admin"));
            await _roleManager.CreateAsync(new IdentityRole("member"));

            return Ok();
        }

        public IActionResult Login()
        {
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Login(AdminLoginViewModel admin, string returnUrl)
        {
            AppUser appUser = await _userManager.FindByNameAsync(admin.UserName);

            if (admin == null || (!await _userManager.IsInRoleAsync(appUser, "admin") && !await _userManager.IsInRoleAsync(appUser, "superadmin")))
            {
                ModelState.AddModelError("", "UserName or Password is not true");
                return View();
            }
            var result = await _signInManager.PasswordSignInAsync(appUser, admin.Password, admin.RememberMe, false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "UserName or Password is not true");
                return View();
            }

            return returnUrl != null ? Redirect(returnUrl) : RedirectToAction("index", "dashboard");
        }
        [Authorize(Roles = "admin,superadmin")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("login", "account");
        }

    }
}

