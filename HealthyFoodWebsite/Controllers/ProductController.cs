using HealthyFoodWebsite.Models;
using HealthyFoodWebsite.Repositories.ProductRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace HealthyFoodWebsite.Controllers
{
    [Authorize(Roles = "BusinessOwner")]
    public class ProductController : Controller, IController.IOperationalController<Product>
    {
        // Object Fields Zone
        private readonly AbstractProductRepository productRepository;


        // Dependency Injection Zone
        public ProductController(AbstractProductRepository productRepository) =>
            this.productRepository = productRepository;


        // Object Methods Zone
        [AllowAnonymous]
        public async Task<IActionResult> GetAllAsync(int productInsertionResult = -1)
        {
            if (User.Identity?.IsAuthenticated == true && productInsertionResult >= 0)
            {
                ViewBag.ProcessSuccessful = productInsertionResult;
            }

            if (productInsertionResult == -1)
            {
                ViewBag.ProcessSuccessful = -1;
            }

            return View("Product", await productRepository.GetAllAsync());
        }

        [AllowAnonymous]
        public async Task<IActionResult> FilterByCategoryAsync(string category)
        {
            return Json(await productRepository.FilterByCategoryAsync(category));
        }


        // Insertion Entrance
        public IActionResult InsertAsync()
        {
            ViewBag.ConfigurationStatus = "Add";
            return View("ConfigureProduct");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InsertAsync([Bind("ImageFile, Name, Price, Category, IsDisplayed")] Product entity)
        {
            ViewBag.ConfigurationStatus = "Add";

            if (entity.ImageFile == null)
            {
                ViewBag.ProductAdded = 0;
                ModelState.AddModelError<Product>(e => e.ImageFile, "The (Product Image) field is required.");
                return View("ConfigureProduct", entity);
            }

            if (ModelState.IsValid)
            {
                if (await productRepository.InsertAsync(entity))
                {
                    ViewBag.ProductAdded = 1;
                }
                else
                {
                    ViewBag.ProductAdded = 0;
                }
            }
            else
            {
                ViewBag.ProductAdded = 0;
            }

            return View("ConfigureProduct", entity);
        }


        // Update Entrance
        public async Task<IActionResult> UpdateAsync(int id)
        {
            ViewBag.ConfigurationStatus = "Update";
            return View("ConfigureProduct", await productRepository.GetByIdAsync(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAsync([Bind("Id, ImageUri, ImageFile, Name, Price, Category, IsDisplayed")] Product entity)
        {
            ViewBag.ConfigurationStatus = "Update";

            var targetProduct = await productRepository.GetByIdAsync(entity.Id);

            if (ModelState.IsValid)
            {
                if (await productRepository.UpdateAsync(entity))
                {
                    ViewBag.ProductUpdated = 1;
                    return View("Product", await productRepository.GetAllAsync());
                }
                else
                {
                    ViewBag.ProductUpdated = 0;
                }
            }
            else
            {
                ViewBag.ProductUpdated = 0;
            }

            return View("ConfigureProduct", targetProduct);
        }


        public async Task<bool> DeactivateAsync(int id)
        {
            var entity = await productRepository.GetByIdAsync(id);
            return await productRepository.DeactivateAsync(entity!);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await productRepository.GetByIdAsync(id);
            return await productRepository.DeleteAsync(entity!);
        }
    }
}
