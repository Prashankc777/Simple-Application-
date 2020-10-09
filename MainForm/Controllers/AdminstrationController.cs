using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using MainForm.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Modals.Models;
using Modals.ViewModels;
using Modals.ViewModels.Adminstration;

namespace MainForm.Controllers
{
    
    //[Authorize(Roles = "admin")]
   [AllowAnonymous]
    public class AdminstrationController : Controller
    {

        #region --------------- GLOBAL VARIABLE ---------------
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        #endregion

        #region --------------- Constructor ---------------
        public AdminstrationController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            this._roleManager = roleManager;
            this._userManager = userManager;
        }
        #endregion


        #region --------------- USERS ---------------

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ListUsers()
        {
            var users = _userManager.Users;
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
            {
                ViewBag.ErrorMessage = $"User with id {id} cannot be found";
                return View("NotFound");
            }
            else
            {
                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View("ListUsers");

            }

        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            if (user is null)
            {
                ViewBag.ErrorMessage = $"User with id {Id} cannot be found";
                return View("NotFound");
            }

            var userclaims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);

            var model = new EditUserViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                City = user.City,
                Claims = userclaims.Select(c => c.Value).ToList(),
                Roles = userRoles
            };

            return View(model);

        }

        [HttpGet]
        public async Task<IActionResult> ManageUserRoles(string UserId)
        {
            ViewBag.UserId = UserId;
            var user = await _userManager.FindByIdAsync(UserId);
            if (user is null)
            {
                ViewBag.ErrorMessage = $"USer with {UserId} cannot be found";  
                return View("NotFound");
            }


            var Model = new List<UserRolesViewModel>();
            foreach (var role in _roleManager.Roles)
            {
                var userRoleViewModel = new UserRolesViewModel()
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };

                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }

                Model.Add(userRoleViewModel);
            }
            return View(Model);
        }
        [HttpPost]
        public async Task<IActionResult> ManageUserRoles(List<UserRolesViewModel> model, string UserId)
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if (user is null)
            {
                ViewBag.ErrorMessage = $"User with id {UserId} cannot be found";
                return View("NotFound");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, roles);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("","cannot remove exisiting Role");
                return View(model);
            }

            result = await _userManager.AddToRolesAsync(user,
                model.Where(x => x.IsSelected).Select(y => y.RoleName));

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected roles to user");
                return View(model);
            }

            return RedirectToAction("EditUser", new { Id = UserId });

        }


        #endregion

        #region -------------- ROLES --------------

        [HttpPost]
        [Authorize(Policy = "DeleteRolePolicy")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role is null)
            {
                ViewBag.ErrorMessage = $"Role with id {id} cannot be found";
                return View("NotFound");
            }
            else
            {
                var result = await _roleManager.DeleteAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View("ListRoles");

            }

        }


        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (!ModelState.IsValid) return View();
            var identityRole = new IdentityRole()
            {
                Name = model.RoleName
            };

            var result = await _roleManager.CreateAsync(identityRole);
            if (result.Succeeded)
            {
                return RedirectToAction("ListRoles", "Adminstration");
            }

            foreach (var identityError in result.Errors)
            {
                ModelState.AddModelError("", identityError.Description);
            }

            return View();
        }

        public IActionResult ListRoles()
        {
            return View(_roleManager.Roles);
        }

        [HttpGet]
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role is null)
            {
                ViewBag.ErrorMessage = $"Role with {id} cannot be found";
            }

            var model = new EditRoleViewModel()
            {
                Id = role.Id,
                RoleName = role.Name
            };

            foreach (var user in _userManager.Users)
            {
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }


            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.Id);
            if (role is null)
            {
                ViewBag.ErrorMessage = $"Role with {model.RoleName} cannot be found";
            }
            else
            {
                role.Name = model.RoleName;
                var result = await _roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles", "Adminstration");
                }

                foreach (var identityError in result.Errors)
                {
                    ModelState.AddModelError("", identityError.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;

            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            var model = new List<UserRoleViewModel>();

            foreach (var user in _userManager.Users)
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };

                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }

                model.Add(userRoleViewModel);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string RoleId)
        {
            var role = await _roleManager.FindByIdAsync(RoleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {RoleId} cannot be found";
                return View("NotFound");
            }

            for (int i = 0; i < model.Count; i++)
            {
                var user = await _userManager.FindByIdAsync(model[i].UserId);

                IdentityResult result = null;

                if (model[i].IsSelected && !(await _userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await _userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && await _userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }

                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                        continue;
                    else
                        return RedirectToAction("EditRole", new { Id = RoleId });
                }
            }

            return RedirectToAction("EditRole", new { Id = RoleId });
        }
        #endregion

        #region -------------- Claims ------------

        [HttpGet]
        public async Task<IActionResult> ManageUserClaims(string UserId)
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if (user is null)
            {
                ViewBag.ErrorMessage = $"User with id not found";
                return View("NotFound");

            }

            var existingUserClaims = await _userManager.GetClaimsAsync(user);
            var model = new UserClaimViewModel()
            {
                UserId = UserId
            };

            foreach (Claim claim in ClaimsStore.AllClaim )
            {
                var userclaim = new UserClaim()
                {
                    ClaimType = claim.Type
                };

                if (existingUserClaims.Any(c=>c.Type == claim.Type))
                {
                    userclaim.IsSelected = true;
                }
                
                model.Claims.Add(userclaim);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageUserClaims(UserClaimViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.UserId} cannot be found";
                return View("NotFound");
            }

            // Get all the user existing claims and delete them
            var claims = await _userManager.GetClaimsAsync(user);
            var result = await _userManager.RemoveClaimsAsync(user, claims);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing claims");
                return View(model);
            }

            // Add all the claims that are selected on the UI
            result = await _userManager.AddClaimsAsync(user,
                model.Claims.Where(c => c.IsSelected).Select(c => new Claim(c.ClaimType, c.ClaimType)));

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected claims to user");
                return View(model);
            }

            return RedirectToAction("EditUser", new { Id = model.UserId });

        }



        #endregion






    }
}
