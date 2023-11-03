using HealthyFoodWebsite.Models;
using HealthyFoodWebsite.Repositories.BlogRepository;
using HealthyFoodWebsite.Repositories.TestimonialRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HealthyFoodWebsite.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        // Object Fields Zone
        private readonly ILogger<HomeController> _logger;
        private readonly AbstractTestimonialRepository testimonialRepository;
        private readonly AbstractBlogPostRepository blogPostRepository;


        // Dependency Injection Zone
        public HomeController(ILogger<HomeController> logger, AbstractTestimonialRepository testimonialRepository, AbstractBlogPostRepository blogPostRepository)
        {
            this.testimonialRepository = testimonialRepository;
            this.blogPostRepository = blogPostRepository;
            _logger = logger;
        }


        // Object Methods Zone
        public async Task<IActionResult> GetViewAsync()
        {
            ViewBag.LastThreePosts = await blogPostRepository.GetLastThreePostsAsync();
            ViewBag.TestimonialsList = await testimonialRepository.GetAllAsync();
            return View("Home");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}