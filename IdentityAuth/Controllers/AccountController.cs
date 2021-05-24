using IdentityAuth.Models;
using IdentityAuth.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityAuth.Controllers
{
    [Authorize(Policy ="AdminPolicy")]
    public class AccountController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AccountController(RoleManager<IdentityRole> userRole, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.roleManager = userRole;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        [HttpGet]
        public async Task<IActionResult> ManageClaims(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            var model = new UserClaimsView();
           
            ViewBag.UserName = user.UserName;
            if (user == null)
            {
                ViewBag.Message = $"User with Id={id} can not found";
                return View("NotFound");
            }
           
            else
            {
                var exisitingClaims = await userManager.GetClaimsAsync(user);
                model.UserId = id;


                foreach (Claim claim in ClaimStore.AllClaims)
                {
                    UserClaim userClaim = new UserClaim()
                    {
                        ClaimType = claim.Type
                    };
                     if(exisitingClaims.Any(c=>c.Type == claim.Type && c.Value == "True"))
                    {
                        userClaim.IsSelected = true;
                    }
                    else
                    {
                        userClaim.IsSelected = false;
                    }
                    model.userClaims.Add(userClaim);
                }
                return View(model);
            }
                
        }

        [HttpPost]
        public async Task<IActionResult> ManageClaims(UserClaimsView model)
        {
            var user = await userManager.FindByIdAsync(model.UserId);
            IdentityResult result = null;
            if (user == null)
            {
                ViewBag.Message = $"User with id {user.Id} can not be found";
                return RedirectToAction("EditUser", "Account", new { id = user.Id });
            }
            var exisitingclaim=await userManager.GetClaimsAsync(user);
            await userManager.RemoveClaimsAsync(user, exisitingclaim);

             result=await userManager.AddClaimsAsync(user,
                model.userClaims.Select(c => new Claim(c.ClaimType,c.IsSelected ? "True":"False")));

            await signInManager.RefreshSignInAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("EditUser", "Account", new { id = user.Id });
            }
            foreach(var err in result.Errors)
            {
                ModelState.AddModelError("", err.Description);
            }
            return View(model);
            
        }
        [Authorize(Policy = "DeleteRolePolicy")]
        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            try
            {
                var result = await roleManager.DeleteAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("RoleList", "Account");
                }

                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
                return RedirectToAction("RoleList", "Account");
            }
            catch (DbUpdateException ex)
            {
                ViewBag.Role = role.Name;
                ViewBag.Message = ex.Message;
                return View("CustomError");
            }

        }
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            var result=await  userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("ListUser", "Account");
            }

            foreach (var err in result.Errors)
            {
                ModelState.AddModelError("", err.Description);
            }
            return RedirectToAction("ListUser", "Account");

        }
        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            var editUser = new EditUserView()
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                City = user.City
            };
            editUser.Roles = await userManager.GetRolesAsync(user);
            var userclaims=await userManager.GetClaimsAsync(user);
            editUser.Claims = userclaims.Select(x => x.Type + "  :  "+x.Value).ToList();
            return View(editUser);
        }
        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserView model)
        {
            var user = await userManager.FindByIdAsync(model.Id);

            user.UserName = model.UserName;
            user.Email = model.Email;
            user.City = model.City;
            var result=await userManager.UpdateAsync(user);

            if(result.Succeeded)
            {
                return RedirectToAction("ListUser", "Account");
            }

            foreach(var err in result.Errors)
            {
                ModelState.AddModelError("", err.Description);
            }
            return View(model);
        }

    
     
        [HttpGet]
        public IActionResult ListUser()
        {
            var user = userManager.Users;
            return View("UserList",user);
        }
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole role = new IdentityRole(model.RoleName);
                IdentityResult result = await roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("RoleList", "Account");
                }
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
            }
            return View(model);
        }

        public IActionResult RoleList()
        {
            var roleList = roleManager.Roles.ToList();
            return View(roleList);
        }
        [Authorize(Policy = "EditRolePolicy")]
        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ViewBag.Message = $"Role with Id={id} can not found";
                return View("NotFound");
            }
            else
            {
                EditViewModel model = new EditViewModel()
                {
                    Id = role.Id,
                    RoleName = role.Name
                };
                foreach (var user in userManager.Users.ToList())
                {
                    if (await userManager.IsInRoleAsync(user, role.Name))
                    {
                        model.Users.Add(user.UserName);
                    }
                }
                return View(model);
            }
        }

       

        [HttpPost]
        public async Task<IActionResult> EditRole(EditViewModel model)
        {
            var role = await roleManager.FindByIdAsync(model.Id);
            if (role == null)
            {
                ViewBag.Message = $"Role with Id={model.Id} can not found";
                return View("NotFound");
            }
            else
            {

                role.Name = model.RoleName;
                var result = await roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("RoleList", "Account");
                }
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }

                return View(model);
            }

        }
        [HttpGet]
        public async Task<IActionResult>  ManageUserRole(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            var userRoles = new List<ManageUserRole>();
            foreach(var role in roleManager.Roles.ToList())
            {
                var managerUserRole = new ManageUserRole()
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };

                if(await userManager.IsInRoleAsync(user,role.Name))
                {
                    managerUserRole.IsSelected = true;
                }
                else
                {
                    managerUserRole.IsSelected = false;
                }
                userRoles.Add(managerUserRole);
            }
            ViewBag.UserName = user.UserName;
            ViewBag.UserId = user.Id;
            return View("ManagerUserRole",userRoles);

        }

        [HttpPost]
        public async Task<IActionResult> ManageUserRole(List<ManageUserRole> model, string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.Message = $"User with id {id} can not be found";
                return RedirectToAction("EditUser", "Account", new { id = id });
            }
            for (int i = 0; i < model.Count; i++)
            {
                var role = await roleManager.FindByIdAsync(model[i].RoleId);
                IdentityResult result = null;
                bool checker = await userManager.IsInRoleAsync(user, role.Name);
                if (model[i].IsSelected && !checker)
                {
                    result = await userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && checker)
                {
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
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
                        return RedirectToAction("EditUser", "Account", new { id = id });
                }
            }
            return RedirectToAction("EditUser", "Account", new { id = id });
        }
        [HttpGet]
        public async Task<IActionResult> UsersRole(string roleId)
        {
            ViewBag.roleId = roleId;
            var roles = await roleManager.FindByIdAsync(roleId);
            ViewBag.roleName = roles.Name;
            if(roles == null)
            {
                ViewBag.Message = $"Role with id {roleId} can not be found";
                return RedirectToAction("EditRole", "Account", roleId);
            }
            var modelList = new List<UserRoleViewModel>();
            foreach (var user in userManager.Users.ToList())
            {
                var userRoleViewModel = new UserRoleViewModel()
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };
                if (await userManager.IsInRoleAsync(user, roles.Name)) 
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }
                modelList.Add(userRoleViewModel);
            }
            return View(modelList);
        }
        [HttpPost]
        public async Task<IActionResult> UsersRole(List<UserRoleViewModel> model,string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            if(role == null)
            {
                ViewBag.Message = $"Role with id {roleId} can not be found";
                return RedirectToAction("EditRole", "Account", new { id = roleId });
            }
            for(int i=0;i<model.Count;i++)
            {
                var user = await userManager.FindByIdAsync(model[i].UserId);
                IdentityResult result = null;
                bool checker = await userManager.IsInRoleAsync(user, role.Name);
                if (model[i].IsSelected && !checker)
                {
                  result= await userManager.AddToRoleAsync(user, role.Name);
                }
                else if ( !model[i].IsSelected && checker)
                {
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }
                if(result.Succeeded)
                {
                    if (i < (model.Count - 1))
                        continue;
                    else
                        return RedirectToAction("EditRole", "Account", new { id = roleId });
                }
            }
            return RedirectToAction("EditRole", "Account", new { id = roleId });
        }
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View("AccessDenied");
        }
    }
}
