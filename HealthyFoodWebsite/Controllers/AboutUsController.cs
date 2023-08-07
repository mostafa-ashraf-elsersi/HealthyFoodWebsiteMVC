using Microsoft.AspNetCore.Mvc;

namespace HealthyFoodWebsite.Controllers
{
    public class AboutUsController : Controller
    {
        // Object Methods Zone
        public IActionResult GetView()
        {
            return View("AboutUs");
        }
    }
}
