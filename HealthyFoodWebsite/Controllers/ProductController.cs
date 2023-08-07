using HealthyFoodWebsite.Controllers.IController;
using HealthyFoodWebsite.Models;
using HealthyFoodWebsite.Repositories.ProductRepository;
using Microsoft.AspNetCore.Mvc;

namespace HealthyFoodWebsite.Controllers
{
    public class ProductController : Controller, IController.IOperationalController<Product>
    {
        // Object Fields Zone
        private readonly AbstractProductRepository productRepository;


        // Dependency Injection Zone
        public ProductController(AbstractProductRepository productRepository) =>
            this.productRepository = productRepository;


        // Object Methods Zone
        public async Task<IActionResult> GetAllAsync()
        {
            return View("Product", await productRepository.GetAllAsync());
        }

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
        public async Task<bool> InsertAsync(Product entity)
        {
            return await productRepository.InsertAsync(entity);
        }


        // Update Entrance
        public async Task<IActionResult> UpdateAsync(int id)
        {
            ViewBag.ConfigurationStatus = "Update";
            return View("ConfigureProduct", await productRepository.GetByIdAsync(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<bool> UpdateAsync(Product entity)
        {
            return await productRepository.UpdateAsync(entity);
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
