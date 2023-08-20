using HealthyFoodWebsite.Controllers.IController;
using HealthyFoodWebsite.Models;
using HealthyFoodWebsite.Repositories.BlogSubscriberRepository;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<bool> InsertAsync(BlogSubscriber entity)
        {
            return await blogSubscriberRepository.InsertAsync(entity);
        }

        public async Task<bool> DeleteSubscriptionsAsync()
        {
            var subscriptions = await blogSubscriberRepository.GetUserSubscriptionsAsync("mostafa_ashraf"); // TODO: Get the correct username.
            return await blogSubscriberRepository.DeleteSubscriptionsAsync(subscriptions);
        }
    }
}
