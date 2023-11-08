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
            ViewBag.PostUpdated = 1;

            return View("BlogGrid", pagesTuple.Item1);
        }

        [AllowAnonymous]
        public async Task<IActionResult> GetBlogPost(int postId)
        {
            return View("BlogPostViewer", await blogRepository.GetByIdAsync(postId));
        }


        // Insertion Entrance
        public IActionResult InsertAsync()
        {
            ViewBag.ConfigurationStatus = "Add";
            return View("ConfigureBlogPost");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InsertAsync([Bind("PosterFile, Title, Content, AuthorType, IsDisplayed")] BlogPost entity)
        {
            ViewBag.ConfigurationStatus = "Add";

            if (entity.PosterFile == null)
            {
                ViewBag.PostAdded = 0;
                ModelState.AddModelError<BlogPost>(e => e.PosterFile, "The (Post Poster) field is required.");
                return View("ConfigureBlogPost", entity);
            }

            if (ModelState.IsValid)
            {
                if (await blogRepository.InsertAsync(entity))
                {
                    ViewBag.PostAdded = 1;
                }
                else
                {
                    ViewBag.PostAdded = 0;
                }
            }
            else
            {
                ViewBag.PostAdded = 0;
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

            var targetPost = await blogRepository.GetByIdAsync(entity.Id);

            if (ModelState.IsValid)
            {
                if (await blogRepository.UpdateAsync(entity))
                {
                    return RedirectToActionPermanentPreserveMethod("GetPages", "BlogPost", new { PageIndex = 0 });
                }
                else
                {
                    ViewBag.PostUpdated = 0;
                }
            }
            else
            {
                ViewBag.PostUpdated = 0;
            }

            return View("ConfigureBlogPost", targetPost);
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
    }
}
