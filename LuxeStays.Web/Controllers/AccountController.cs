using LuxeStays.Application.Common.Interfaces;
using LuxeStays.Application.Common.Utility;
using LuxeStays.Domain.Entities;
using LuxeStays.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace LuxeStays.Web.Controllers
{
    public class AccountController:Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<AccountController> logger
            )
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger; // Add this line

        }

        public IActionResult Login(string returnUrl=null)
        {
            returnUrl ??= Url.Content("~/");
            LoginVM loginVM = new LoginVM()
            {
                RedirectUrl = returnUrl,
            };
            return View(loginVM);
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (ModelState.IsValid) { 
                var result = await _signInManager.PasswordSignInAsync(loginVM.Email,loginVM.Password,loginVM.RememberMe,lockoutOnFailure:false);
                if (result.Succeeded)
                {
                  
                    if (string.IsNullOrEmpty(loginVM.RedirectUrl))
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return LocalRedirect(loginVM.RedirectUrl);
                    }
                }
            } else
            {
                ModelState.AddModelError("", "Invalid login attempt.");
            }
            return View(loginVM);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index","Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult Register(string returnUrl=null)
        {
            returnUrl ??= Url.Content("~/");
           
            if (!_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).Wait();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).Wait();
            }
            RegisterVM RegisterVM = new RegisterVM()
            {
                RoleList = _roleManager.Roles.Select(item => new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Name
                }),
                RedirectUrl = returnUrl
               
            };

            return View(RegisterVM);
        }

        [HttpPost]
        // public async Task<IActionResult> Register(RegisterVM registerVM)
        //{
        //    ApplicationUser user = new()
        //    {
        //        Name = registerVM.Name,
        //        Email = registerVM.Email,
        //        UserName = registerVM.Email,
        //        PhoneNumber = registerVM.PhoneNumber,
        //        NormalizedEmail = registerVM.Email.ToUpper(),
        //        CreatedAt = DateTime.Now,
        //        EmailConfirmed = true
        //    };
        //    var result = await _userManager.CreateAsync(user, registerVM.Password);

        //    if (result.Succeeded)
        //    {
        //        if (!string.IsNullOrEmpty(registerVM.Role))
        //        {
        //            await _userManager.AddToRoleAsync(user, registerVM.Role);
        //        }

        //        else
        //        {
        //            await _userManager.AddToRoleAsync(user, SD.Role_Customer);

        //        }

        //        await _signInManager.SignInAsync(user, isPersistent: false);


        //        if (string.IsNullOrEmpty(registerVM.RedirectUrl))
        //        {
        //            return RedirectToAction("Index", "Home");
        //        }
        //        else
        //        {
        //            return LocalRedirect(registerVM.RedirectUrl);
        //        }
        //    }


        //    foreach (var error in result.Errors)
        //    {
        //        ModelState.AddModelError("", error.Description);
        //    }


        //    registerVM.RoleList = _roleManager.Roles.Select(item => new SelectListItem
        //    {
        //        Text = item.Name,
        //        Value = item.Name
        //    });


        //    return View(registerVM);
        //}
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            _logger.LogInformation("Starting user registration for email: {Email}", registerVM.Email);

            ApplicationUser user = new()
            {
                Name = registerVM.Name,
                Email = registerVM.Email,
                UserName = registerVM.Email,
                PhoneNumber = registerVM.PhoneNumber,
                NormalizedEmail = registerVM.Email.ToUpper(),
                CreatedAt = DateTime.Now,
                EmailConfirmed = true
            };

            _logger.LogDebug("Attempting to create user: {Email}", registerVM.Email);
            var result = await _userManager.CreateAsync(user, registerVM.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("User {Email} created successfully.", registerVM.Email);

                string role = !string.IsNullOrEmpty(registerVM.Role) ? registerVM.Role : SD.Role_Customer;
                _logger.LogDebug("Adding user to role: {Role}", role);

                await _userManager.AddToRoleAsync(user, role);
                _logger.LogInformation("User {Email} added to role {Role}.", registerVM.Email, role);

                await _signInManager.SignInAsync(user, isPersistent: false);
                _logger.LogInformation("User {Email} signed in.", registerVM.Email);

                string redirectUrl = string.IsNullOrEmpty(registerVM.RedirectUrl) ?
                    Url.Action("Index", "Home") : registerVM.RedirectUrl;

                _logger.LogInformation("Redirecting user to: {RedirectUrl}", redirectUrl);
                return LocalRedirect(redirectUrl);
            }
            else
            {
                // 🔹 Fixed: Changed {UserEmail} to {Email} for consistency
                _logger.LogError("Failed to create user {Email}. Errors: {@Errors}",
                    registerVM.Email, result.Errors);

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            registerVM.RoleList = _roleManager.Roles.Select(item => new SelectListItem
            {
                Text = item.Name,
                Value = item.Name
            });

            return View(registerVM);
        }
    }
}
