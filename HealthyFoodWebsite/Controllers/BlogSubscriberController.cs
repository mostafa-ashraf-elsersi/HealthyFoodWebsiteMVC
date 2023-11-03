using HealthyFoodWebsite.Controllers.IController;
using HealthyFoodWebsite.Models;
using HealthyFoodWebsite.Repositories.BlogSubscriberRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HealthyFoodWebsite.Controllers
{
    public class BlogSubscriberController : Controller, IOperationalController<BlogSubscriber>
    {
        // Object Fields Zone
        private readonly AbstractBlogSubscriberRepository blogSubscriberRepository;


        // Dependency Injection Zone
        public BlogSubscriberController(AbstractBlogSubscriberRepository blogSubscriberRepository) =>
            this.blogSubscriberRepository = blogSubscriberRepository;


        // Object Methods Zone
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<bool> InsertAsync([Bind("Username, EmailAddress")] BlogSubscriber entity)
        {
            if (User.Identity?.IsAuthenticated == true && (User.IsInRole("BusinessOwner") || User.IsInRole("User")))
                if (ModelState.IsValid)
                    return await blogSubscriberRepository.InsertAsync(entity);
                else
                    return false;
            else
                return false;
        }

        [Authorize(Roles = "BusinessOwner, User")]
        public async Task<bool> DeleteSubscriptionsAsync()
        {
            var subscriptions = await blogSubscriberRepository.GetUserSubscriptionsAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            return await blogSubscriberRepository.DeleteSubscriptionsAsync(subscriptions);
        }
    }
}
