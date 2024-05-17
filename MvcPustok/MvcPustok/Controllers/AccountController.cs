using System;
using System.Net.Mail;
using System.Security.Claims;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MvcPustok.Data;
using MvcPustok.Models;
using MvcPustok.ViewModels;

namespace MvcPustok.Controllers
{
	public class AccountController:Controller
	{
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(AppDbContext context,UserManager<AppUser> userManager,SignInManager<AppUser> signInManager)
		{
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Register()
        {
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Register(MemberRegisterViewModel member)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            if (_userManager.Users.Any(x => x.NormalizedEmail == member.Email.ToUpper()))
            {
                ModelState.AddModelError("Email", "Email is already taken");
                return View();
            }
            AppUser appUser = new AppUser()
            {
                UserName = member.UserName,
                Email = member.Email,
                FullName = member.FullName
            };
            var result = await _userManager.CreateAsync(appUser, member.Password);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    if (item.Code == "DuplicateUserName")
                        ModelState.AddModelError("UserName", "UserName is already taken");
                    else
                    ModelState.AddModelError("", item.Description);

                }
                return View();
            }
            await _userManager.AddToRoleAsync(appUser, "member");
            return RedirectToAction("login", "account");
        }
        public IActionResult Login()
        {
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Login(MemberLoginViewModel member, string returnUrl)
        {
            if (member.Password == null)
            {
                ModelState.AddModelError("Password", "Password mustn't be null");
                return View(member);
            }

            AppUser appUser = await _userManager.FindByEmailAsync(member.Email);

            if (appUser == null || !await _userManager.IsInRoleAsync(appUser, "member"))
            {
                ModelState.AddModelError("", "Email or Password is incorrect");
                return View(member);
            }

            var result = await _signInManager.PasswordSignInAsync(appUser, member.Password, false, true);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Email or Password is incorrect");
                return View(member);
            }

            return returnUrl != null ? Redirect(returnUrl) : RedirectToAction("index", "home");
        }

        [Authorize(Roles ="member")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }
        [Authorize(Roles = "member")]
        public async Task<IActionResult> Profile(string tab = "dashboard")
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("login", "account");
            }

            AppUser? user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("login", "account");
            }

            ProfileViewModel profileViewModel = new ProfileViewModel()
            {
                ProfileEditView = new ProfileEditViewModel()
                {
                    FullName = user.FullName,
                    UserName = user.UserName,
                    Email = user.Email
                },
                Orders = _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Book)
                    .Where(o => o.AppUserId == user.Id)
                    .ToList()
            };
            ViewBag.Tab = tab;
            return View(profileViewModel);
        }

        [ValidateAntiForgeryToken]
        [Authorize(Roles = "member")]
        [HttpPost]
        public async Task<IActionResult> Profile(ProfileEditViewModel profileEditView, string tab = "profile")
        {
            ViewBag.Tab = tab;
            ProfileViewModel profileViewModel = new ProfileViewModel();
            profileViewModel.ProfileEditView = profileEditView;

            if (!ModelState.IsValid)
            {
                TempData["ToastMessage"] = "Something went wrong";
                TempData["ToastType"] = "error";
                return View(profileViewModel);
            }
            AppUser? user = await _userManager.GetUserAsync(User);

            user.UserName = profileEditView.UserName;
            user.Email = profileEditView.Email;
            user.FullName = profileEditView.FullName;

            if (_userManager.Users.Any(x => x.Id != User.FindFirstValue(ClaimTypes.NameIdentifier) && x.NormalizedEmail == profileEditView.Email.ToUpper()))
            {
                ModelState.AddModelError("Email", "Email is already taken");
                TempData["ToastMessage"] = "Email is already taken";
                TempData["ToastType"] = "error";
                return View(profileViewModel);
            }

            if (profileEditView.NewPassword != null)
            {
                var passwordResult = await _userManager.ChangePasswordAsync(user, profileEditView.CurrentPassword, profileEditView.NewPassword);

                if (!passwordResult.Succeeded)
                {
                    foreach (var err in passwordResult.Errors)
                        ModelState.AddModelError("", err.Description);

                    TempData["ToastMessage"] = "Something went wrong.";
                    TempData["ToastType"] = "error";
                    return View(profileViewModel);
                }
            }

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                {
                    if (err.Code == "DuplicateUserName")
                        ModelState.AddModelError("UserName", "UserName is already taken");
                    else
                        ModelState.AddModelError("", err.Description);
                }
                TempData["ToastMessage"] = "Something went wrong";
                TempData["ToastType"] = "error";
                return View(profileViewModel);
            }
            await _signInManager.SignInAsync(user, false);
            TempData["ToastMessage"] = "Profile updated successfully.";
            TempData["ToastType"] = "success";
            return View(profileViewModel);
        }
        [Authorize(Roles = "member")]
        public IActionResult OrderDetails(int id)
        {
            
            var order = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                return RedirectToAction("notfound","error"); 
            }
            return View(order);
        }

        public IActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ForgetPassword(ForgetPasswordViewModel viewmodel)
        {
            if (!ModelState.IsValid) return View(viewmodel);

            AppUser? user = _userManager.FindByEmailAsync(viewmodel.Email).Result;

            if (user == null || !_userManager.IsInRoleAsync(user, "member").Result)
            {
                ModelState.AddModelError("", "Account isn't found");
                return View();
            }

            var token = _userManager.GeneratePasswordResetTokenAsync(user).Result;


            var url = Url.Action("verify", "account", new { email = viewmodel.Email, token = token }, Request.Scheme);
            TempData["EmailSent"] = viewmodel.Email;


            //var emailMessage = new MimeMessage();
            //emailMessage.From.Add(MailboxAddress.Parse("elmar@gmail.com"));
            //emailMessage.To.Add(MailboxAddress.Parse(viewmodel.Email));
            //emailMessage.Subject = "Reset Password";
            //emailMessage.Body = new TextPart("html")
            //{
            //    Text = $"<a href='{url}'>Reset Password</a>"
            //};
            //using (var client = new MailKit.Net.Smtp.SmtpClient())
            //{
            //    client.Connect("elmar@gmail.com", 587, true); 
            //    client.Authenticate("celestino.jakubowski@ethereal.email", "qdD9uFrHFWdaTYBSQb");
            //    client.Send(emailMessage);
            //    client.Disconnect(true);
            //}

            //SocketException: nodename nor servname provided, or not known

            return Json(new { url = url });
        }
        public IActionResult Verify(string email, string token)
        {
            AppUser? user = _userManager.FindByEmailAsync(email).Result;

            if (user == null || !_userManager.IsInRoleAsync(user, "member").Result)
            {
                return RedirectToAction("notfound", "error");
            }

            if (!_userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", token).Result)
            {
                return RedirectToAction("notfound", "error");
            }

            TempData["email"] = email;
            TempData["token"] = token;

            return RedirectToAction("resetPassword");
        }
        public IActionResult ResetPassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordViewModel viewmodel)
        {
            AppUser? user = _userManager.FindByEmailAsync(viewmodel.Email).Result;

            if (user == null || !_userManager.IsInRoleAsync(user, "member").Result)
            {
                ModelState.AddModelError("", "Account isn't found");
                return View();
            }

            if (!_userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", viewmodel.Token).Result)
            {
                ModelState.AddModelError("", "Account isn't found");
                return View();
            }

            var result = _userManager.ResetPasswordAsync(user, viewmodel.Token, viewmodel.NewPassword).Result;

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View();
            }

            return RedirectToAction("login");
        }


    }
}


