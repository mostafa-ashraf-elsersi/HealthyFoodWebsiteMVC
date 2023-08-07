using HealthyFoodWebsite.Models;
using HealthyFoodWebsite.Repositories.ShoppingBag;
using Microsoft.AspNetCore.Mvc;

namespace HealthyFoodWebsite.Controllers
{
    public class ShoppingBagController : Controller, IController.IOperationalController<ShoppingBagItem>
    {
        // Object Fields Zone
        private readonly AbstractShoppingBagRepository shoppingBagRepository;


        // Dependency Injection Zone
        public ShoppingBagController(AbstractShoppingBagRepository shoppingBagRepository) =>
            this.shoppingBagRepository = shoppingBagRepository;


        // Object Methods Zone
        public async Task<IActionResult> GetUserShoppingBagItemsAsync()
        {
            return View("ShoppingBag", await shoppingBagRepository.GetUserShoppingBagItemsAsync());
        }

        public async Task<ShoppingBagItem?> GetByIdAsync(int id)
        {
            return await shoppingBagRepository.GetByIdAsync(id);
        }

        public async Task<bool> InsertUsingProductAsync(int productId)
        {
            return await shoppingBagRepository.InsertUsingProductAsync(productId);
        }

        [HttpPost]
        public async Task<bool> UpdateUsingJsonObjectsArrayAsync(string itemsArray)
        {
            return await shoppingBagRepository.UpdateUsingJsonObjectsArrayAsync(itemsArray);
        }

        public async Task<bool> DeleteAsync(ShoppingBagItem entity)
        {
            return await shoppingBagRepository.DeleteAsync(entity);
        }
    }
}
