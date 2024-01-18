using IdentityDemoProject.Models;
using IdentityDemoProject.Models.ViewModels;
using IdentityDemoProject.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace IdentityDemoProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailSender _emailSender;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        public IActionResult Register(string returnurl = null)
        {
            ViewData["ReturnUrl"] = returnurl;
            RegisterViewModel registerViewModel = new RegisterViewModel();
            return View(registerViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnurl = null)
        {
            ViewData["ReturnUrl"] = returnurl;
            returnurl = returnurl ?? Url.Content("/");

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, Name = model.UserName };

                var result = await _userManager.CreateAsync(user, model.Password);

                if(result.Succeeded)
                {
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userid = user.Id, code }, protocol: HttpContext.Request.Scheme);

                    await _emailSender.SendEmailAsync(model.Email, "Confirm email",
                        $"Please, confirm your email by clicking here: <a href='{callbackUrl}'>link</a>");

                    return RedirectToAction(nameof(ConfirmEmailSent));
                    //await _signInManager.SignInAsync(user, isPersistent: false);
                    //return LocalRedirect(returnurl);
                }

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(RegisterViewModel model)
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnurl = null)
        {
            ViewData["ReturnUrl"] = returnurl;
            returnurl = returnurl ?? Url.Content("/");

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password,isPersistent: model.RememberMe, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    return LocalRedirect(returnurl);
                }

                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "You try to access with a wrong password so many times. Try again in 1 minute");
                }

                ModelState.AddModelError(string.Empty, "Invalid username or password");
            }

            return View(model);
        }

        public IActionResult Login(string returnurl = null)
        {
            ViewData["ReturnUrl"] = returnurl;
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    //await _emailSender.SendEmailAsync(model.Email, "Reset Password Testing", "<h1>Como andamios?</h1>");

                    var code = await _userManager.GeneratePasswordResetTokenAsync(user);

                    var callbackUrl = Url.Action("ResetPassword", "Account", new { userid = user.Id, code }, protocol: HttpContext.Request.Scheme);

                    await _emailSender.SendEmailAsync(model.Email, "Reset password", 
                        $"Please, reset your password by clicking here: <a href='{callbackUrl}'>link</a>");

                    return RedirectToAction("ForgotPasswordConfirmation", "Account");
                }
            }
            
            ModelState.AddModelError(string.Empty, $"There is no user with email: {model.Email} registered");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    return RedirectToAction(nameof(ResetPasswordConfirmation));
                }

                var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(ResetPasswordConfirmation));
                } else
                {
                    ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description);
                }

            }
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? View("Error") : View();
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ConfirmEmail(string code = null)
        {
            return code == null ? View("Error") : View();
        }

        [HttpGet]
        public IActionResult ConfirmEmailSent()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation(string code = null)
        {
            return View();
        }
    }
}
