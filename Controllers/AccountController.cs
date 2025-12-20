using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ConferenceRoomBooking.Models;
using ConferenceRoomBooking.ViewModels;

namespace ConferenceRoomBooking.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        
        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        
        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    UserName = model.Email,
                    Email = model.Email
                };
                
                var result = await _userManager.CreateAsync(user, model.Password);
                
                if (result.Succeeded)
                {
                    // Assign "User" role to new users
                    await _userManager.AddToRoleAsync(user, "User");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            
            return View(model);
        }
        
        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        
        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email, 
                    model.Password, 
                    model.RememberMe, 
                    lockoutOnFailure: false);
                
                if (result.Succeeded)
                {
                    return RedirectToLocal(returnUrl);
                }
                
                ModelState.AddModelError(string.Empty, "Nieprawidłowy email lub hasło");
            }
            
            return View(model);
        }
        
        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        
        // GET: /Account/AccessDenied
        public IActionResult AccessDenied()
        {
            return View();
        }
        
        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}