using Autofac;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using StudentLockerSystem.Entities;
using StudentLockerSystem.Models;
using System.Text;

namespace StudentLockerSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<Student> _signInManager;
        private readonly UserManager<Student> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<AccountController> _logger;
        private readonly ILifetimeScope _scope;

        public AccountController(
            UserManager<Student> userManager,
            SignInManager<Student> signInManager,
            RoleManager<ApplicationRole> roleManager,
            ILogger<AccountController> logger,
            ILifetimeScope scope)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
            _scope = scope;
        }

        public async Task<IActionResult> Register(string? returnUrl = null)
        {
            var model = _scope.Resolve<RegisterViewModel>();
            model.ReturnUrl = returnUrl;
            model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            model.ReturnUrl ??= Url.Content("~/");
            model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var studentFolder = Path.Combine("wwwroot", model.FirstName);
                Directory.CreateDirectory(studentFolder);

                var user = new Student
                {
                    UserName = model.FirstName,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    FolderPath = studentFolder
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {

                    _logger.LogInformation("User created a new account with password.");
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToAction("RegisterConfirmation", new { UserName = model.FirstName, returnUrl = model.ReturnUrl }); ;
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(model.ReturnUrl);
                    }


                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public async Task<IActionResult> Login(string? returnUrl = null)
        {
            var model = _scope.Resolve<LoginViewModel>();

            model.ReturnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            model.ReturnUrl ??= Url.Content("~/");

            model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(model.UserName);
                    var claims = (await _userManager.GetClaimsAsync(user)).ToArray();

                    return LocalRedirect(model.ReturnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction("LoginWith2fa", new { ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToAction("Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToAction("AccessDenied");
            }
        }
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
        //public async Task<IActionResult> Profile(LoginViewModel model)
        //{
        //    var student = await _userManager.FindByNameAsync(model.Email);
        //    return View(student);
        //}
        [AllowAnonymous]
        public IActionResult ConfirmEmail()
        {
            return View();
        }
    }
}
