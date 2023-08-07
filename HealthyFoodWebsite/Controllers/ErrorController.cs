using Microsoft.AspNetCore.Mvc;

namespace HealthyFoodWebsite.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult GetView()
        {
            return View("Error");
        }
    }
}
