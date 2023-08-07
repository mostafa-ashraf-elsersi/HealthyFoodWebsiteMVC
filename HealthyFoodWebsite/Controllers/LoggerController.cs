using HealthyFoodWebsite.Models;
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


        // Dependency Injection Zone
        public LoggerController(AbstractLoggerRepository loggerRepository) =>
            this.loggerRepository = loggerRepository;


        // Object Methods Zone
        public async Task<IActionResult> GetAllAdminsAsync()
        {
            return View("AdminsGrid", await loggerRepository.GetAllAdminsAsync());
        }

        public async Task<Logger?> GetByIdAsync(int id)
        {
            return await loggerRepository.GetByIdAsync(id);
        }

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

        public async Task<bool> UpdateAsync(Logger entity)
        {
            return await loggerRepository.UpdateAsync(entity);
        }

        public async Task<bool> DeactivateAsync(Logger entity)
        {
            return await loggerRepository.DeactivateAsync(entity);
        }

        public async Task<bool> DeleteAsync(Logger entity)
        {
            return await loggerRepository.DeleteAsync(entity);
        }
    }
}
