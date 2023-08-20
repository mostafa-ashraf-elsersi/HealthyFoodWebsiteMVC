using HealthyFoodWebsite.Models;
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
        public async Task<IActionResult> GetAllAsync()
        {
            return View("UsersTestimonials", await testimonialRepository.GetAllAsync());
        }

        public async Task<IActionResult> GetUserTestimonialsAsync()
        {
            return View("UserTestimonial", await testimonialRepository.GetUserTestimonialsAsync());

        }


        // Insertion Entrance
        public IActionResult GetView()
        {
            return View("AddTestimonial");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<bool> InsertAsync(Testimonial entity)
        {
            return await testimonialRepository.InsertAsync(entity);
        }


        public async Task<bool> ToggleSeenOrUnseenAsync(int id)
        {
            var entity = await testimonialRepository.GetByIdAsync(id);
            return await testimonialRepository.ToggleSeenOrUnseenAsync(entity!);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await testimonialRepository.GetByIdAsync(id);
            return await testimonialRepository.DeleteAsync(entity!);
        }
    }
}
