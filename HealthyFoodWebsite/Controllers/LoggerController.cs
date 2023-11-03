using HealthyFoodWebsite.EmailTemplate;
using HealthyFoodWebsite.Models;
using HealthyFoodWebsite.Repositories.BlogSubscriberRepository;
using HealthyFoodWebsite.Repositories.LoggerRepository;
using HealthyFoodWebsite.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HealthyFoodWebsite.Controllers
{
    public class LoggerController : Controller, IController.IOperationalController<Logger>
    {
        // Object Fields Zone
        private readonly AbstractLoggerRepository loggerRepository;
        private readonly AbstractBlogSubscriberRepository blogSubscriberRepository;
        private readonly EmailFactory emailObject;


        // Dependency Injection Zone
        public LoggerController(AbstractLoggerRepository loggerRepository, AbstractBlogSubscriberRepository blogSubscriberRepository, EmailFactory emailObject)
        {
            this.loggerRepository = loggerRepository;
            this.blogSubscriberRepository = blogSubscriberRepository;
            this.emailObject = emailObject;
        }


        // Object Methods Zone
        [Authorize(Roles = "BusinessOwner")]
        public async Task<IActionResult> GetAllAdminsAsync()
        {
            return View("AdminsGrid", await loggerRepository.GetAllAdminsAsync());
        }


        // Login Entrance
        [AllowAnonymous]
        public IActionResult LogIn()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AuthenticateLoggerAsync([Bind("Username, Password")] LoggerCredentialsParameters credentialsParameters)
        {
            var logger = await loggerRepository.CheckSystemLoggerExistenceAsync(credentialsParameters.Username,
                                                                                 credentialsParameters.Password);

            if (logger is not null)
            {
                if (logger.IsActive == true)
                {
                    ClaimsIdentity claimsIdentity = new(CookieAuthenticationDefaults.AuthenticationScheme);

                    List<Claim> claims = new()
                    {
                        new Claim(ClaimTypes.SerialNumber, logger.Id.ToString()),
                        new Claim(ClaimTypes.NameIdentifier, logger.Username),
                        new Claim(ClaimTypes.Role, logger.Role)
                    };

                    claimsIdentity.AddClaims(claims);

                    ClaimsPrincipal claimsPrincipal = new(claimsIdentity);

                    var authenticationProperties = new AuthenticationProperties()
                    {
                        ExpiresUtc = DateTimeOffset.Now.AddDays(1),
                        IsPersistent = true
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                                  claimsPrincipal,
                                                  authenticationProperties);

                    return RedirectToActionPermanent("GetView", "Home");
                }
                else
                {
                    ModelState.AddModelError("DeactivatedLogger", "You're not allowed to log into the system with your account, contact business owner for this issue.");
                    return View("LogIn", credentialsParameters);
                }
                    
            }
            
            ModelState.AddModelError("InvalidCredentials", "Wrong username or password!");
            return View("LogIn", credentialsParameters);
        }

        [Authorize(Roles = "BusinessOwner, Admin, User")]
        public async Task SignOutSystemLoggerAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }


        // Insertion Entrance
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InsertAsync([Bind("Username, FullName, Email, Password, ConfirmingPassword, PhoneNumber, Role")] Logger entity)
        {
            if (ModelState.IsValid)
            {
                await loggerRepository.InsertAsync(entity);
                return RedirectToAction("GetView", "Home");
            }
            else
                return View("Register", entity);
        }


        // Update Entrance
        [Authorize(Roles = "BusinessOwner, User")]
        public async Task<IActionResult> UpdateAsync(int id)
        {
            var entity = await loggerRepository.GetByIdAsync(id);

            if (entity?.Role != "Admin")
                ViewBag.AccountTypeWord = "Your";
            else
                ViewBag.AccountTypeWord = "Admin";

            if (await blogSubscriberRepository.SearchForSubscriptionsOfUsername(entity!.Username))
                ViewBag.HasBlogSubscriptions = true;
            else
                ViewBag.HasBlogSubscriptions = false;

            return View("EditProfile", entity);
        }

        [Authorize(Roles = "BusinessOwner, User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<bool> UpdateAsync(Logger entity)
        {
            if (ModelState.IsValid)
            {
                var loggerAccount = await loggerRepository.GetByIdAsync(entity.Id);

                if (loggerAccount!.Role == "BusinessOwner" || loggerAccount.Role == "User")
                {
                    entity.Password = loggerAccount.Password;
                }

                return await loggerRepository.UpdateAsync(entity);
            }
            return false;
        }


        // Password Change Zone
        [Authorize(Roles = "BusinessOwner, User")]
        public IActionResult PasswordChangePortal()
        {
            return View();
        }

        // TODO: Authorization
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PasswordChangePage([Bind("Password")] LoggerCredentialsParameters credentialsParameters)
        {
            var logger = await loggerRepository.GetByIdAsync(int.Parse(User.FindFirstValue(ClaimTypes.SerialNumber)!));

            if (logger!.Password == credentialsParameters.Password)
            {
                return View();
            }
            else
            {
                ModelState.AddModelError("InvalidPassword", "Invalid password!");
                return View("PasswordChangePortal", credentialsParameters);
            }
        }

        // TODO: Commented until the emailing section is fixed.
        // TODO: Authorization
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<bool> OverwriteOldPasswordWithNewOne([Bind("Password, ConfirmingPassword")] Logger logger)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var _logger = await loggerRepository.GetByIdAsync(1); // TODO: Get the real logger Id here.

        //        _logger!.Password = logger.Password;
        //    }
        //}


        // Password Forgetting Zone
        [AllowAnonymous]
        public IActionResult PasswordForgettingIdentityConfirmationPortal()
        {
            return View();
        }

        // TODO: Authorization
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendEmailWithLinkOfPasswordChangePage([Bind("EmailAddress")] LoggerCredentialsParameters credentialsParameters)
        {
            var isEmailExisted = await loggerRepository.CheckSystemLoggerEmailExistence(credentialsParameters.EmailAddress);

            if (isEmailExisted == true)
            {
                await emailObject.SendEmailWithLinkOfPasswordChangePage(credentialsParameters.EmailAddress);
                return RedirectToActionPermanent("GetView", "Home");
            }
            else
            {
                ModelState.AddModelError("InvalidEmailAddress", "Invalid email address!");
                return View("PasswordForgettingIdentityConfirmationPortal", credentialsParameters);
            }
        }


        [Authorize(Roles = "BusinessOwner")]
        public async Task<bool> DeactivateAsync(int id)
        {
            var entity = await loggerRepository.GetByIdAsync(id);
            return await loggerRepository.DeactivateAsync(entity!);
        }

        [Authorize(Roles = "BusinessOwner, User")]
        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await loggerRepository.GetByIdAsync(id);
            return await loggerRepository.DeleteAsync(entity!);
        }
    }
}
