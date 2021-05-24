using IdentityAuth.Entity;
using IdentityAuth.Models;
using IdentityAuth.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityAuth.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly AppDbContext appDbContext;

        public HomeController(ILogger<HomeController> logger,UserManager<ApplicationUser> userManager
                                    ,SignInManager<ApplicationUser> signInManager,AppDbContext appDbContext)
        {
            _logger = logger;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.appDbContext = appDbContext;
        }
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await userManager.GetUserAsync(User);
            var editUser = new EditUserView()
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                City = user.City
            };

            editUser.Roles = await userManager.GetRolesAsync(user);
            return View(editUser);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Edit(int id)
        {
            var model = appDbContext.customers.Where(item => item.Id == id).First();
            return View(model);
        }
        [HttpGet]
        public IActionResult Details(int id)
        {
            var model = appDbContext.customers.Where(item => item.Id == id).First();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> LogOut()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index");
        }

        public IActionResult Index()
        {
           var modelList= appDbContext.customers.ToList();
            return View("CustomerInfoView",modelList);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginUser user, string returnUrl)
        {
            if (ModelState.IsValid)
            {
               
                var res = await signInManager.PasswordSignInAsync(user.Email,user.Password,user.RememberMe,false);
                if (res.Succeeded)
                {
                    if ( !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);
                
                    else
                        return RedirectToAction("Index");
                }
               
                 ModelState.AddModelError("", "Invalid Username or Password");
                

            }
            return View();
        }
       
        [AcceptVerbs("Get","Post")]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailInUse(string Email)
        {
            var user = await userManager.FindByEmailAsync(Email);
            if(user == null)
            {
                return Json(true);
            }
            return Json($"Email {Email} already in use");
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View("RegisterView");
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterUser user)
        {
            if(ModelState.IsValid)
            {
                var result = new ApplicationUser { UserName = user.Email, Email = user.Email ,City=user.City};
                var res=await userManager.CreateAsync(result, user.Password);
                IdentityUser temp = new IdentityUser();
               
                if(res.Succeeded)
                {
                    var roleassign=await userManager.AddToRoleAsync(result, "User");
                   if(signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                    {
                        return RedirectToAction("ListUser", "Account");
                    }
                    await signInManager.SignInAsync(result, isPersistent: false);
                    return RedirectToAction("Index");
                }
                foreach(var error in res.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
               
            }
            return View("RegisterView");
        }
        public IActionResult Privacy()
        {
            
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
