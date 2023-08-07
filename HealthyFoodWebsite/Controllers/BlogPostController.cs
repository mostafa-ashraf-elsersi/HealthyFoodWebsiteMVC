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

        public IActionResult ConfigureBlogPost()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<bool> InsertAsync(BlogPost entity)
        {
            return await blogRepository.InsertAsync(entity);
        }

        public async Task<bool> UpdateAsync(BlogPost entity)
        {
            return await blogRepository.UpdateAsync(entity);
        }

        public async Task<bool> DeactivateAsync(BlogPost entity)
        {
            return await blogRepository.DeactivateAsync(entity);
        }

        public async Task<bool> DeleteAsync(BlogPost entity)
        {
            return await blogRepository.DeleteAsync(entity);
        }

        public async Task<IActionResult> GetBlogPost(int postId)
        {
            return View("BlogPostViewer", await blogRepository.GetByIdAsync(postId));
        }
    }
}
