using HealthyFoodWebsite.Controllers.IController;
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
        public async Task<IActionResult> GetAllAsync()
        {
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
                ModelState.AddModelError<Product>(e => e.ImageFile, "The (Product Image) field is required.");
                return View("ConfigureProduct", entity);
            }

            if (ModelState.IsValid)
            {
                await productRepository.InsertAsync(entity);
                return View("ConfigureProduct");
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

            if (ModelState.IsValid)
            {
                await productRepository.UpdateAsync(entity);
                return View("Product");
            }

            return View("ConfigureProduct", entity);
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
