using HealthyFoodWebsite.Models;
using HealthyFoodWebsite.Repositories.TestimonialRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthyFoodWebsite.Controllers
{
    [Authorize(Roles = "User")]
    public class TestimonialController : Controller, IController.IOperationalController<Testimonial>
    {
        // Object Fields Zone
        private readonly AbstractTestimonialRepository testimonialRepository;


        // Dependency Injection Zone
        public TestimonialController(AbstractTestimonialRepository testimonialRepository) =>
            this.testimonialRepository = testimonialRepository;


        // Object Methods Zone
        [AllowAnonymous]
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
        public async Task<IActionResult> InsertAsync([Bind("Content, RatingValue")] Testimonial entity)
        {
            if (entity.Content is null || entity.Content == "")
            {
                ModelState.AddModelError("EmptyContent","The field (Content) is required.");
                return View("AddTestimonial", entity);
            }

            if (entity.Content.Length < 20 || entity.Content.Length > 150)
            {
                ModelState.AddModelError("InvalidStringLength", "The field (Content) must be of length between 20 characters at min and 150 characters at max.");
                return View("AddTestimonial", entity);
            }

            if (entity.RatingValue is null)
            {
                ModelState.AddModelError("RequiredRatingValue", "The field (Rate Us) is required.");
                return View("AddTestimonial", entity);
            }

            if (entity.RatingValue < 1 || entity.RatingValue > 5)
            {
                ModelState.AddModelError("InvalidRatingValue", "Invalid rating value! Stop messing around it in the DOM tree.");
                return View("AddTestimonial", entity);
            }

            await testimonialRepository.InsertAsync(entity);
            return View("UserTestimonial", await testimonialRepository.GetUserTestimonialsAsync());
        }


        [Authorize(Roles = "BusinessOwner")]
        public async Task<bool> ToggleSeenOrUnseenAsync(int id)
        {
            var entity = await testimonialRepository.GetByIdAsync(id);
            return await testimonialRepository.ToggleSeenOrUnseenAsync(entity!);
        }

        [Authorize(Roles = "BusinessOwner, User")]
        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await testimonialRepository.GetByIdAsync(id);
            return await testimonialRepository.DeleteAsync(entity!);
        }
    }
}
