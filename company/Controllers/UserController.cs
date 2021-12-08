using company.Models.Settings;
using company.Models.UsersSec;
using company.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace company.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IMailService mailService;

        public UserController(UserManager<AppUser> userManager, IMailService mailService)
        {
            this.userManager = userManager;
            this.mailService = mailService;
        }
        public async Task<IActionResult> Index()
        {
            AppUser user = await userManager.FindByNameAsync(User.Identity.Name);
            ViewBag.User = user.FullName;
            return View();
        }
        [AllowAnonymous]
        public IActionResult Anonymous()
        {
            return View();
        }

        public IActionResult ChangePassword()
        {
            return View(new PasswordView());
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromForm]PasswordView model)
        {
            if (ModelState.IsValid)
            {
                if (model.Password == model.RetypePassword)
                {
                    AppUser user = await userManager.FindByNameAsync(User.Identity.Name);
                    bool isValid = await userManager.CheckPasswordAsync(user, model.CurrentPassword);
                    if (isValid)
                    {
                        IdentityResult result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.Password);
                        if (result.Succeeded)
                        {
                            return View("ChangePasswordConfirmation");
                        }
                    }
                }
            }
            return View();
        }

        public IActionResult ChangeEmail()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangeEmail(string email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                AppUser user = await userManager.FindByNameAsync(User.Identity.Name);
                string token = await userManager.GenerateChangeEmailTokenAsync(user, email);

                string link  = Url.Action("ChangeEmailConfirmation",
                    "User", new
                    {
                        token = token,
                        email = email
                    },
                    protocol: HttpContext.Request.Scheme);

                MailRequest req = new MailRequest
                {
                    ToEMail = email,
                    Subject = "Token For Changing Email",
                    Body = "Herer Is Your Token <a style=\"color:Green; font-weight: 900;\" href=\"" + link + "\">Confirm Email Changing</a></p>"
                };
                await mailService.SendEmailAsync(req);
                return View("verifiemail");

            }
            return View(email);
        }
        public async Task<IActionResult> ChangeEmailConfirmation(string token, string email)
        {
            if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(email))
            {
                AppUser user = await userManager.FindByNameAsync(User.Identity.Name);
                IdentityResult result = await userManager.ChangeEmailAsync(user, email, token);
                if (result.Succeeded)
                {
                    return View();
                }
            }
            return RedirectToAction("ConfirmEmail");
        }
    }
}
