using company.Models;
using company.Models.Settings;
using company.Models.UsersSec;
using company.Services;
using company.Views.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace company.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IMailService mailService;
        private readonly IReCaptchaService reCaptchaService;

        public AccountController(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager, 
            RoleManager<IdentityRole> roleManager, 
            IMailService mailService,
            IReCaptchaService reCaptchaService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.mailService = mailService;
            this.reCaptchaService = reCaptchaService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult>  Register([FromForm] UserView model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser
                {
                    UserName = model.UserName,
                    FullName = model.FullName,
                    Email = model.Email
                };
                IdentityResult result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    IdentityResult result2 = await userManager.AddToRoleAsync(user, "Member");
                    if (result2.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                }
            }
            return View(model);
        }
        public IActionResult Login(string returnUrl)
        {
            
            ViewBag.returnUrl = returnUrl;
            
            return View(new LoginView());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromForm] LoginView model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await userManager.FindByNameAsync(model.UserName);
                if (user != null)
                {
                    await signInManager.SignOutAsync();
                    Microsoft.AspNetCore.Identity.SignInResult result = await signInManager.PasswordSignInAsync(user, model.Password, false, false);
                    if (result.Succeeded)
                    {
                        return Redirect(returnUrl ?? "/account/");
                    }
                    else
                    {
                        ModelState.AddModelError(nameof(LoginView.Password), "Wrong Password.");
                    }
                }
                else
                {
                    ModelState.AddModelError(nameof(LoginView.UserName), "Invalid user/password or account is not activated yet.");
                }
            }
            return View(model);
        }
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return View();
        }
        public async Task<IActionResult> GenerateRoles()
        {
            string[] roleNames = { "Admin", "Manager", "Member" };
            IdentityResult roleResult;
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
            return View();
        }
        public async Task<IActionResult> GenerateUsers()
        {
            AppUser userAva1 = await userManager.FindByNameAsync("Manager");
            if (userAva1 == null)
            {
                AppUser user1 = new AppUser
                {
                    UserName = "manager",
                    FullName = "Manager",
                    Email = "manager@gmail.com",
                    EmailConfirmed = true
                };

                IdentityResult result = await userManager.CreateAsync(user1, "manager");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user1, "Manager");
                }
            }

            AppUser userAva2 = await userManager.FindByNameAsync("Admin");
            if (userAva1 == null)
            {
                AppUser user1 = new AppUser
                {
                    UserName = "admin",
                    FullName = "Admin",
                    Email = "admin@gmail.com",
                    EmailConfirmed = true
                };

                IdentityResult result = await userManager.CreateAsync(user1, "admin");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user1, "Admin");
                }
            }
            return View();
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
        public async Task<IActionResult> SendEmail()
        {
            MailRequest req = new MailRequest();
            req.ToEMail = "moh.ja.ps@gmail.com";
            req.Subject = "Mail Test";
            req.Body = "This Is A Message";
            await mailService.SendEmailAsync(req);
            ViewBag.mail = req.ToEMail;
            return View();

        }
        public IActionResult Register2()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> Register2([FromForm]UserView model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser();
                user.UserName = model.UserName;
                user.FullName = model.FullName;
                user.Email = model.Email;

                IdentityResult result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    IdentityResult result2 = await userManager.AddToRoleAsync(user, "Member");
                    if (!result2.Succeeded)
                    {
                        ModelState.AddModelError(nameof(LoginView.UserName), "Falied To Create A User.");
                        return View(model);
                    }

                    var usr = await userManager.FindByEmailAsync(model.Email);
                    var userId = await userManager.GetUserIdAsync(user);
                    var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    string link = Url.Action("ConiformEmail", "Account", new
                    {
                        userid = userId,
                        code = code
                    },protocol:HttpContext.Request.Scheme);


                    MailRequest req = new MailRequest();
                    req.ToEMail = user.Email;
                    req.Subject = "Email Confirmation";
                    req.Body = "<p>Click This Url To Activate Your Account: <href a='" + link + "'>" + link + "</a></p>";
                    await mailService.SendEmailAsync(req);
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(nameof(LoginView.UserName), "Failed To Create An User.");
                }
                
            }
            return View(model);
        }
        public async Task<IActionResult> ConiformEmail(string code, string userid)
        {
            var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

            var user = await userManager.FindByIdAsync(userid);

            IdentityResult result = await userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                ViewBag.status = "Email Confirmation Was Succed";
            }
            else
            {
                ViewBag.status = "Email Confirmation Was Failed Or Invalid Token ";
            }
            return View();
        }
        public IActionResult RegisterCaptcha()
        {
            ViewData["ReCaptchaKey"] = reCaptchaService.Config.Key;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RegisterCaptcha([FromForm]UserView model)
        {
            ViewData["ReCaptchaKey"] = reCaptchaService.Config.Key;
            if (ModelState.IsValid)
            {
                string token = Request.Form["g-recaptcha-response"];
                if (!reCaptchaService.ValidateRecaptcha(token))
                {
                    ModelState.AddModelError(nameof(UserView.UserName), "You Failed CAPTCHA");
                    return View(model);
                }

                AppUser user = new AppUser();
                user.UserName = model.UserName;
                user.FullName = model.FullName;
                user.Email = model.Email;

                IdentityResult result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    IdentityResult result2 = await userManager.AddToRoleAsync(user, "Member");
                    if (!result2.Succeeded)
                    {
                        ModelState.AddModelError(nameof(LoginView.UserName), "Falied To Create A User.");
                        return View(model);
                    }

                    var usr = await userManager.FindByEmailAsync(model.Email);
                    var userId = await userManager.GetUserIdAsync(user);
                    var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    string link = Url.Action("ConiformEmail", "Account", new
                    {
                        userid = userId,
                        code = code
                    }, protocol: HttpContext.Request.Scheme);


                    MailRequest req = new MailRequest();
                    req.ToEMail = user.Email;
                    req.Subject = "Email Confirmation";
                    req.Body = "<p>Click This Url To Activate Your Account: <href a='" + link + "'>" + link + "</a></p>";
                    await mailService.SendEmailAsync(req);
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(nameof(LoginView.UserName), "Failed To Create An User.");
                }
            }
            return View();
        }
        public async Task<IActionResult> LoginRememberMe(string returnUrl)
        {

            ViewBag.returnUrl = returnUrl;
            return View(new LoginView());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginRememberMe([FromForm] LoginView model, bool rememberMe, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await userManager.FindByNameAsync(model.UserName);
                if (user != null)
                {
                    bool isValid = await userManager.CheckPasswordAsync(user, model.Password);
                    if (isValid)
                    {
                        var claims = new List<Claim>();

                        claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
                        claims.Add(new Claim(ClaimTypes.Name, user.UserName));

                        var roles = await userManager.GetRolesAsync(user);
                        foreach (var role in roles)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role));
                        }

                        var identity = new ClaimsIdentity(
                            claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        var principal = new ClaimsPrincipal(identity);

                        var authProperities = new AuthenticationProperties();
                        authProperities.IsPersistent = rememberMe;
                        authProperities.ExpiresUtc = DateTimeOffset.Now.AddDays(3);

                        HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperities).Wait();

                        return Redirect(returnUrl ?? "/account/");

                    }
                    else
                    {
                        ModelState.AddModelError(nameof(LoginView.Password), "Wrong Password.");
                    }
                }
                else
                {
                    ModelState.AddModelError(nameof(LoginView.UserName), "Invalid user/password or account is not activated yet.");
                }
            }
            return View(model);
        }
        public async Task<IActionResult> LogoutRememberMe()
        {
            await HttpContext.SignOutAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme);
            return View();
        }
        public IActionResult ForgotAccount()
        {
            return View(new ForgotAccount());
        }
        [HttpPost]
        public async Task<IActionResult> ForgotAccount([FromForm]ForgotAccount model)
        {
            if (!string.IsNullOrEmpty(model.Email))
            {
                AppUser user = await userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    MailRequest req = new MailRequest();
                    req.ToEMail = user.Email;
                    req.Subject = "Forgot Account";
                    req.Body = $"<p>Here Is Your Account <span style=\"color: blue; font-weight:900;\">{user.UserName}</span></p>";
                    await mailService.SendEmailAsync(req);
                    return View("ForgotAccountConfirmation");
                }
            }
            return View(model);
        }
        public IActionResult ForgotPassword()
        {
            return View(new ForgotAccount());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword([FromForm]ForgotAccount model)
        {
            if (!string.IsNullOrEmpty(model.UserName))
            {
                AppUser user = await userManager.FindByNameAsync(model.UserName);
                if (user != null)
                {
                    string token = await userManager.GeneratePasswordResetTokenAsync(user);
                    string link = Url.Action(
                        "ResetPassword",
                        "Account",
                        new
                        {
                            username = model.UserName,
                            token = token
                        },
                        protocol: HttpContext.Request.Scheme
                        );

                    MailRequest req = new MailRequest
                    {
                        ToEMail = user.Email,
                        Subject = "Foregot Password",
                        Body = "Herer Is Your Token <a style=\"color:red; font-weight: 900;\" href=\"" + link + "\">Confirm Email Changing</a></p>"
                    };
                    await mailService.SendEmailAsync(req);
                    return View("ForgotPasswordCongirmation");
                }
                return View(model);
            }
            return View();
        }
        public IActionResult ResetPassword(string username, string token)
        {
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(token))
            {
                ViewBag.UserName = username;
                ViewBag.Token = token;
                return View();
            }
            return RedirectToAction("ForgotPassword");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string username, string token, string password)
        {
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(password))
            {
                AppUser user = await userManager.FindByNameAsync(username);
                if (user != null)
                {
                    IdentityResult result = await userManager.ResetPasswordAsync(user, token, password);
                    if (result.Succeeded)
                    {
                        return View("ResetPasswordCongirmation");
                    }
                    ViewBag.UserName = username;
                    ViewBag.Token = token;
                }
            }
            return View();
        }




         
    }
}
