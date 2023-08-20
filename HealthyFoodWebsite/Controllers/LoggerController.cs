using HealthyFoodWebsite.Models;
using HealthyFoodWebsite.Repositories.BlogSubscriberRepository;
using HealthyFoodWebsite.Repositories.LoggerRepository;
using HealthyFoodWebsite.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HealthyFoodWebsite.Controllers
{
    public class LoggerController : Controller, IController.IOperationalController<Logger>
    {
        // Object Fields Zone
        private readonly AbstractLoggerRepository loggerRepository;
        private readonly AbstractBlogSubscriberRepository blogSubscriberRepository;


        // Dependency Injection Zone
        public LoggerController(AbstractLoggerRepository loggerRepository, AbstractBlogSubscriberRepository blogSubscriberRepository)
        {
            this.loggerRepository = loggerRepository;
            this.blogSubscriberRepository = blogSubscriberRepository;
        }


        // Object Methods Zone
        public async Task<IActionResult> GetAllAdminsAsync()
        {
            return View("AdminsGrid", await loggerRepository.GetAllAdminsAsync());
        }


        // Login Entrance
        public IActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<bool> AuthenticateLoggerAsync(LoggerCredentialsParameters credentialsParameters)
        {
            var logger = await loggerRepository.CheckSystemLoggerExistence(credentialsParameters.Username,
                                                                                 credentialsParameters.Password);
            // TODO: Prevent deactivated loggers from logging into the system.

            if (logger is not null)
            {
                ClaimsIdentity claimsIdentity = new(CookieAuthenticationDefaults.AuthenticationScheme);

                List<Claim> claims = new()
                {
                    new Claim(ClaimTypes.Name, logger.FullName),
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

                return true;
            }

            return false;
        }

        public async Task SignOutSystemLogger()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }


        // Insertion Entrance
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<bool> InsertAsync(Logger entity)
        {
            return await loggerRepository.InsertAsync(entity);
        }


        // Update Entrance
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<bool> UpdateAsync(Logger entity)
        {
            return await loggerRepository.UpdateAsync(entity);
        }


        public async Task<bool> DeactivateAsync(int id)
        {
            var entity = await loggerRepository.GetByIdAsync(id);
            return await loggerRepository.DeactivateAsync(entity!);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await loggerRepository.GetByIdAsync(id);
            return await loggerRepository.DeleteAsync(entity!);
        }
    }
}
