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
        public async Task<bool> InsertAsync(string emailAddress)
        {
            if (User.Identity?.IsAuthenticated == true && (User.IsInRole("BusinessOwner") || User.IsInRole("User")))
            {
                return await blogSubscriberRepository.InsertAsync(new BlogSubscriber { UserName = User.FindFirstValue(ClaimTypes.NameIdentifier)!, EmailAddress = emailAddress});
            }
            else
            {
                return false;
            }
        }

        [Authorize(Roles = "BusinessOwner, User")]
        public async Task<bool> DeleteSubscriptionsAsync()
        {
            var subscriptions = await blogSubscriberRepository.GetUserSubscriptionsAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            return await blogSubscriberRepository.DeleteSubscriptionsAsync(subscriptions);
        }
    }
}
