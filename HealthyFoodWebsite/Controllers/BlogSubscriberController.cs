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
        public async Task<BlogSubscriber?> GetByIdAsync(int id)
        {
            return await blogSubscriberRepository.GetByIdAsync(id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<bool> InsertAsync(BlogSubscriber entity)
        {
            return await blogSubscriberRepository.InsertAsync(entity);
        }

        public async Task<bool> DeleteAsync(BlogSubscriber entity)
        {
            return await blogSubscriberRepository.DeleteAsync(entity);
        }
    }
}
