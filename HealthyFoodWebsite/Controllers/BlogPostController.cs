using HealthyFoodWebsite.Models;
using HealthyFoodWebsite.Repositories.BlogRepository;
using Microsoft.AspNetCore.Mvc;

namespace HealthyFoodWebsite.Controllers
{
    public class BlogPostController : Controller, IController.IOperationalController<BlogPost>
    {
        // Object Fields Zone
        private readonly AbstractBlogPostRepository blogRepository;


        // Dependency Injection Zone
        public BlogPostController(AbstractBlogPostRepository blogRepository) =>
            this.blogRepository = blogRepository;


        // Object Methods Zone
        public async Task<IActionResult> GetPagesAsync(int pageIndex = 0)
        {
            var pagesTuple = await blogRepository.GetPagesWithCountAsync(pageIndex);

            ViewBag.PagesCount = pagesTuple.Item2;

            return View("BlogGrid", pagesTuple.Item1);
        }

        public async Task<BlogPost?> GetByIdAsync(int id)
        {
            return await blogRepository.GetByIdAsync(id);
        }

        // Insertion Entrance
        public IActionResult ConfigureBlogPost()
        {
            ViewBag.ConfigurationStatus = "Add";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<bool> InsertAsync(BlogPost entity)
        {
            return await blogRepository.InsertAsync(entity);
        }


        // Update Entrance
        public async Task<IActionResult> UpdateAsync(int id)
        {
            ViewBag.ConfigurationStatus = "Update";
            return View("ConfigureBlogPost", await blogRepository.GetByIdAsync(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<bool> UpdateAsync(BlogPost entity)
        {
            return await blogRepository.UpdateAsync(entity);
        }


        public async Task<bool> DeactivateAsync(int id)
        {
            var entity = await blogRepository.GetByIdAsync(id);
            return await blogRepository.DeactivateAsync(entity!);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await blogRepository.GetByIdAsync(id);
            return await blogRepository.DeleteAsync(entity!);
        }

        public async Task<IActionResult> GetBlogPost(int postId)
        {
            return View("BlogPostViewer", await blogRepository.GetByIdAsync(postId));
        }
    }
}
