using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Modals.ViewModels;

namespace MainForm.Controllers
{
    public class AccountController : Controller
    {
        public readonly SignInManager<IdentityUser> SignInManager;
        public readonly UserManager<IdentityUser> UserManager;

        public AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
           SignInManager = signInManager;
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
                if (SignInManager.IsSignedIn(User))
                {
                    return RedirectToAction("Index", "Employee");
                }

                await SignInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Employee");
            }

            foreach (var VARIABLE in result.Errors)
            {
                ModelState.AddModelError("",VARIABLE.Description);
            }

            return View();
        }
    }
}
