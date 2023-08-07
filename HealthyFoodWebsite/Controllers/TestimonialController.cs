using HealthyFoodWebsite.Models;
using HealthyFoodWebsite.Repositories.BlogRepository;
using HealthyFoodWebsite.Repositories.TestimonialRepository;
using Microsoft.AspNetCore.Mvc;

namespace HealthyFoodWebsite.Controllers
{
    public class TestimonialController : Controller, IController.IOperationalController<Testimonial>
    {
        // Object Fields Zone
        private readonly AbstractTestimonialRepository testimonialRepository;


        // Dependency Injection Zone
        public TestimonialController(AbstractTestimonialRepository testimonialRepository) =>
            this.testimonialRepository = testimonialRepository;


        // Object Methods Zone

        public IActionResult GetView()
        {
            return View("AddTestimonial");
        }

#pragma warning disable CS1998
        public async Task<IActionResult> GetAllAsync()
#pragma warning restore CS1998
        {
            throw new Exception("Action Seems To Be Not Needed!");
            //return View(await testimonialRepository.GetAllAsync());
        }

        public async Task<IActionResult> GetUserTestimonialsAsync()
        {
            return View("UserTestimonial", await testimonialRepository.GetUserTestimonialsAsync());

        }

        public async Task<Testimonial?> GetByIdAsync(int id)
        {
            return await testimonialRepository.GetByIdAsync(id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<bool> InsertAsync(Testimonial entity)
        {
            return await testimonialRepository.InsertAsync(entity);
        }

        public async Task<bool> DeleteAsync(Testimonial entity)
        {
            return await testimonialRepository.DeleteAsync(entity);
        }
    }
}
