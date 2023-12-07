using HealthyFoodWebsite.EmailTemplate;
using HealthyFoodWebsite.Models;
using HealthyFoodWebsite.Repositories.BlogSubscriberRepository;
using HealthyFoodWebsite.Repositories.LoggerRepository;
using HealthyFoodWebsite.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;
using System.Text.RegularExpressions;

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



        // LOGIN/LOGOUT ZONE
        [AllowAnonymous]
        public IActionResult LogIn(int accountRegistered = -1)
        {
            if (accountRegistered == 1)
            {
                ViewBag.AccountRegistered = accountRegistered;
            }

            ViewBag.PasswordChanged = -1;
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
                        ExpiresUtc = DateTimeOffset.Now.AddDays(5),
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
        public async Task<IActionResult> SignOutSystemLoggerAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToActionPermanentPreserveMethod("GetView", "Home");
        }



        // REGISTRATION ZONE
        [AllowAnonymous]
        public IActionResult Register()
        {
            ViewBag.AccountRegistered = -1;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InsertAsync([Bind("Username, FullName, Email, Password, ConfirmingPassword, PhoneNumber, Role")] Logger entity)
        {
            if (ModelState.IsValid)
            {
                if (await loggerRepository.InsertAsync(entity))
                {
                    ViewBag.AccountRegistered = 1;
                    return RedirectToActionPermanent("LogIn", new { ViewBag.AccountRegistered });
                }
            }

            ViewBag.AccountRegistered = 0;
            return View("Register", entity);
        }



        // INFORMATION UPDATE ZONE
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
        public async Task<IActionResult> UpdateAsync(Logger entity)
        {
            if (ModelState.IsValid)
            {
                var loggerAccount = await loggerRepository.GetByIdAsync(entity.Id);

                if (loggerAccount!.Role == "BusinessOwner" || loggerAccount.Role == "User")
                {
                    entity.Password = loggerAccount.Password;
                }

                if (await loggerRepository.UpdateAsync(entity))
                {
                    ViewBag.ProfileUpdated = 1;

                    if (loggerAccount.Role == "Admin")
                    {
                        if (loggerAccount.Role != entity.Role)
                        {
                            return RedirectToActionPermanentPreserveMethod("GetAllAdmins", "Logger");
                        }
                    }
                }
                else
                {
                    ViewBag.ProfileUpdated = 0;
                }
            }
            else
            {
                ViewBag.ProfileUpdated = 0;
            }

            if ((await loggerRepository.GetByIdAsync(entity.Id))!.Role == "Admin")
            {
                ViewBag.AccountTypeWord = "Admin";
                entity.Role = "Admin";
            }

            return View("EditProfile", entity);
        }



        // PASSWORD CHANGE ZONE
        [Authorize(Roles = "BusinessOwner, User")]
        public IActionResult PasswordChangePortal()
        {
            return View();
        }

        [Authorize(Roles = "BusinessOwner, User")]
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
                ModelState.AddModelError<LoggerCredentialsParameters>(parameter => parameter.Password, "Invalid password!");
                return View("PasswordChangePortal");
            }
        }

        [Authorize(Roles = "BusinessOwner, User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OverwriteOldPasswordWithNewOne([Bind("Password, ConfirmingPassword")] Logger logger)
        {
            if (logger.Password == logger.ConfirmingPassword)
            {
                if (Regex.IsMatch(logger.Password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$"))
                {
                    var _logger = await loggerRepository.GetByIdAsync(int.Parse(User.FindFirstValue(ClaimTypes.SerialNumber)!));

                    _logger!.Password = logger.Password;

                    if (await loggerRepository.UpdateAsync(_logger))
                    {
                        return RedirectToActionPermanentPreserveMethod("GetView", "Home", new { PasswordChanged = 1 });
                    }
                }

                ModelState.AddModelError<Logger>(logger => logger.Password, "An error occurred! Entered password does not go along with the demonstrated password writing rules!");
            }

            ViewBag.PasswordChanged = 0;
            ModelState.AddModelError<Logger>(logger => logger.Password, "An error occurred! Entered passwords do not match with each other!");
            return View("PasswordChangePage");
        }



        // PASSWORD FORGETTING ZONE
        [AllowAnonymous]
        public IActionResult PasswordForgettingIdentityConfirmationPortal()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendEmailWithLinkOfPasswordChangePage([Bind("EmailAddress")] LoggerCredentialsParameters credentialsParameters)
        {
            var isEmailExisted = await loggerRepository.CheckSystemLoggerEmailExistence(credentialsParameters.EmailAddress);

            if (isEmailExisted == true)
            {
                TempData["EmailAddress"] = credentialsParameters.EmailAddress;

                try
                {
                    await emailObject.SendEmailWithLinkOfPasswordChangePage(credentialsParameters.EmailAddress);
                }
                catch
                {
                }

                return View("PasswordChangeEmailSendingNotification");
            }
            else
            {
                ModelState.AddModelError("InvalidEmailAddress", "Invalid email address!");
                return View("PasswordForgettingIdentityConfirmationPortal", credentialsParameters);
            }
        }

        [AllowAnonymous]
        public IActionResult AssignNewPasswordToSystemLogger()
        {
            return View("NewPasswordAssigningPage");
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignNewPasswordToSystemLogger([Bind("Password, ConfirmingPassword")] Logger logger)
        {
            var emailAddress = TempData["EmailAddress"];

            if (emailAddress != null)
            {
                if (logger.Password == logger.ConfirmingPassword)
                {
                    if (Regex.IsMatch(logger.Password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$"))
                    {
                        var _logger = await loggerRepository.GetLoggerByEmailAddress(emailAddress!.ToString()!);

                        _logger!.Password = logger.Password;

                        await loggerRepository.UpdateAsync(_logger);

                        ViewBag.PasswordChanged = 1;

                        return View("LogIn");
                    }

                    ModelState.AddModelError<Logger>(logger => logger.Password, "An error occurred! Entered password does not go along with the demonstrated password writing rules!");
                }
                else
                {
                    ModelState.AddModelError<Logger>(logger => logger.Password, "An error occurred! Entered passwords do not match with each other!");
                }
            }

            ViewBag.PasswordChanged = 0;

            return View("LogIn");
        }



        // DEACTIVATION AND DELETION ZONE
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
