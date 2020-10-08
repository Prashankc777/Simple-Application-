using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;
using MainForm.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Modals.ViewModels;

namespace MainForm.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        public readonly UserManager<ApplicationUser> UserManager;

        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {

            _signInManager = signInManager;
            UserManager = userManager;
        }


        

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        public async Task<IActionResult> IsEmailInUse(string Email)
        {
            var EmailUser = await UserManager.FindByEmailAsync(Email);
            if (EmailUser is null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Email {Email} is already in use ");
            }
        }


        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel register)
        {
            if (!ModelState.IsValid) return View();
            var user = new ApplicationUser()
            {
                UserName = register.Email,
                Email = register.Email,
                City = register.City
            };
            var result = await UserManager.CreateAsync(user, register.Password);
            if (result.Succeeded)
            {
                if (_signInManager.IsSignedIn(User) && User.IsInRole("Administration"))
                {
                    return RedirectToAction("ListUsers", "Adminstration");
                }


                return RedirectToAction("Login", "Account");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(String.Empty, error.Description);
            }

            return View();

        }
       





    [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await _signInManager.PasswordSignInAsync(
                model.Email, model.Password, model.RememberMe, false);

            if (result.Succeeded)
            {
                if ((!string.IsNullOrEmpty(returnUrl)))
                {
                    return RedirectToAction(returnUrl);

                }

                return RedirectToAction("index", "Employee");
            }

            ModelState.AddModelError(string.Empty, "Invalid Login Attempt");

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
