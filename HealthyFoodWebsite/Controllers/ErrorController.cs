using Microsoft.AspNetCore.Mvc;

namespace HealthyFoodWebsite.Controllers
{
    public class ErrorController : Controller
    {
        // Object Methods Zone
        public IActionResult GetView()
        {
            return View("Error");
        }
    }
}
