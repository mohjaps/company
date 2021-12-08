using company.Models.UsersSec;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace company.Controllers
{
    [Authorize(Roles = "Manager, Admin")]
    public class ManagerController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;

        public ManagerController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> List()
        {
            var admins = await userManager.GetUsersInRoleAsync("Admin");
            var managers = await userManager.GetUsersInRoleAsync("Manager");

            var list = userManager.Users.ToList().Except(admins);
            list = list.Except(managers);

            return View(list);
        }

        public async Task<IActionResult> Details(string id)
        {
            AppUser user = await userManager.FindByIdAsync(id);
            return View(user);
        }
        
        public async Task<IActionResult> Delete(string id)
        {
            AppUser user = await userManager.FindByIdAsync(id);
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id, string btn)
        {
            AppUser user = await userManager.FindByIdAsync(id);
            if (btn == "Delete")
            {
                await userManager.DeleteAsync(user);
                if (user.UserName != "admin" && user.UserName != "manager")
                {
                    return RedirectToAction("List");
                }
                await signInManager.SignOutAsync();
                return RedirectToAction("Index");
                
            }
            return View(user);
        }
    }
}
