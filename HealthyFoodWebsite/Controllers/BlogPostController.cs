using HealthyFoodWebsite.Models;
using HealthyFoodWebsite.Repositories.BlogRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace HealthyFoodWebsite.Controllers
{
    [Authorize(Roles = "BusinessOwner")]
    public class BlogPostController : Controller, IController.IOperationalController<BlogPost>
    {
        // Object Fields Zone
        private readonly AbstractBlogPostRepository blogRepository;


        // Dependency Injection Zone
        public BlogPostController(AbstractBlogPostRepository blogRepository) =>
            this.blogRepository = blogRepository;


        // Object Methods Zone
        [AllowAnonymous]
        public async Task<IActionResult> GetPagesAsync(int pageIndex = 0)
        {
            var pagesTuple = await blogRepository.GetPagesWithCountAsync(pageIndex);

            ViewBag.PagesCount = pagesTuple.Item2;

            return View("BlogGrid", pagesTuple.Item1);
        }


        // Insertion Entrance
        public IActionResult ConfigureBlogPost()
        {
            ViewBag.ConfigurationStatus = "Add";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InsertAsync([Bind("PosterFile, Title, Content, AuthorType, IsDisplayed")] BlogPost entity)
        {
            ViewBag.ConfigurationStatus = "Add";

            if (entity.PosterFile == null)
            {
                ModelState.AddModelError<BlogPost>(e => e.PosterFile, "The (Post Poster) field is required.");
                return View("ConfigureBlogPost", entity);
            }

            if (ModelState.IsValid)
            {
                await blogRepository.InsertAsync(entity);
                return View("ConfigureBlogPost");
            }

            return View("ConfigureBlogPost", entity);
        }


        // Update Entrance
        public async Task<IActionResult> UpdateAsync(int id)
        {
            ViewBag.ConfigurationStatus = "Update";
            return View("ConfigureBlogPost", await blogRepository.GetByIdAsync(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAsync([Bind("Id, PosterUri, PosterFile, Title, Content, AuthorType, IsDisplayed")] BlogPost entity)
        {
            ViewBag.ConfigurationStatus = "Update";

            if (ModelState.IsValid)
            {
                await blogRepository.UpdateAsync(entity);
                return View("BlogGrid");
            }

            return View("ConfigureBlogPost", entity);
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

        [AllowAnonymous]
        public async Task<IActionResult> GetBlogPost(int postId)
        {
            return View("BlogPostViewer", await blogRepository.GetByIdAsync(postId));
        }
    }
}
