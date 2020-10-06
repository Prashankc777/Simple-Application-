using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Modals.ViewModels;

namespace MainForm.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
       
       
        private readonly SignInManager<IdentityUser> _signInManager;
        public readonly UserManager<IdentityUser> UserManager;

        public AccountController(SignInManager<IdentityUser> SignInManager, UserManager<IdentityUser> userManager)
        {
          
            _signInManager = SignInManager;
            UserManager = userManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel register)
        {
            if (!ModelState.IsValid) return View();
            var user = new IdentityUser( userName: register.Email);
            var result = await UserManager.CreateAsync(user, register.Password);
            if (result.Succeeded)
            {
                if (_signInManager.IsSignedIn(User))
                {
                    return RedirectToAction("Index", "Employee");
                }

               
            }
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Employee");

            foreach (var VARIABLE in result.Errors)
            {
                ModelState.AddModelError("",VARIABLE.Description);
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
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

                return RedirectToAction("index", "home");
            }

            ModelState.AddModelError(string.Empty, "Invalid Login Attempt");

            return View(model);
        }
    }
}
