using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthyFoodWebsite.Controllers
{
    [AllowAnonymous]
    public class AboutUsController : Controller
    {
        // Object Methods Zone
        public IActionResult GetView()
        {
            return View("AboutUs");
        }
    }
}
