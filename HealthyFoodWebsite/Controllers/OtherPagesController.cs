using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthyFoodWebsite.Controllers
{
    [AllowAnonymous]
    public class OtherPagesController : Controller
    {
        // Object Methods Zone
        public IActionResult PrivacyPolicy()
        {
            return View();
        }

        public IActionResult TermsAndConditions()
        {
            return View();
        }

        public IActionResult Faqs()
        {
            return View();
        }
    }
}
