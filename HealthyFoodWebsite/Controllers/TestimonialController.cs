using HealthyFoodWebsite.Models;
using HealthyFoodWebsite.Repositories.TestimonialRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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
        [AllowAnonymous]
        public async Task<IActionResult> GetAllAsync()
        {
            return View("UsersTestimonials", await testimonialRepository.GetAllAsync());
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetUserTestimonialsAsync()
        {
            return View("UserTestimonial", await testimonialRepository.GetUserTestimonialsAsync());
        }


        // Insertion Entrance
        [Authorize(Roles = "User")]
        public IActionResult GetView()
        {
            ViewBag.TestimonialAdded = -1;
            return View("AddTestimonial");
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InsertAsync([Bind("Content, RatingValue")] Testimonial entity)
        {
            if (entity.Content is null || entity.Content == "")
            {
                ModelState.AddModelError<Testimonial>(testimonial => testimonial.Content, "The field (Content) is required.");
                ViewBag.TestimonialAdded = 0;
                return View("AddTestimonial", entity);
            }

            if (entity.Content.Length < 20 || entity.Content.Length > 150)
            {
                ModelState.AddModelError<Testimonial>(testimonial => testimonial.Content, "The field (Content) must be of length between 20 characters at min and 150 characters at max.");
                ViewBag.TestimonialAdded = 0;
                return View("AddTestimonial", entity);
            }

            if (entity.RatingValue is null)
            {
                ModelState.AddModelError<Testimonial>(testimonial => testimonial.RatingValue, "The field (Rate Us) is required.");
                ViewBag.TestimonialAdded = 0;
                return View("AddTestimonial", entity);
            }

            if (entity.RatingValue < 1 || entity.RatingValue > 5)
            {
                ModelState.AddModelError<Testimonial>(testimonial => testimonial.RatingValue, "Invalid rating value! Stop messing around it in the DOM tree.");
                ViewBag.TestimonialAdded = 0;
                return View("AddTestimonial", entity);
            }

            if (await testimonialRepository.InsertAsync(entity))
            {
                ViewBag.TestimonialAdded = 1;

            }
            else
            {
                ViewBag.TestimonialAdded = 0;
            }

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
