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
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;

        public AdminController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
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
            var Members = await userManager.GetUsersInRoleAsync("Member");

            var list = userManager.Users.ToList().Except(Members);

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
