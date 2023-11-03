using HealthyFoodWebsite.Models;
using HealthyFoodWebsite.Repositories.OrderRepository;
using HealthyFoodWebsite.Repositories.ShoppingBag;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthyFoodWebsite.Controllers
{
    [Authorize(Roles = "User")]
    public class ShoppingBagController : Controller, IController.IOperationalController<ShoppingBagItem>
    {
        // Object Fields Zone
        private readonly AbstractShoppingBagRepository shoppingBagRepository;
        private readonly AbstractOrderRepository orderRepository;


        // Dependency Injection Zone
        public ShoppingBagController(AbstractShoppingBagRepository shoppingBagRepository, AbstractOrderRepository orderRepository)
        {
            this.shoppingBagRepository = shoppingBagRepository;
            this.orderRepository = orderRepository;
        }

        // Object Methods Zone
        public async Task<IActionResult> GetUserActiveShoppingBagItemsAsync()
        {
            ViewBag.UserConfirmedOrders = await orderRepository.GetUserViewConfirmedOrdersAsync();
            ViewBag.UserConfirmedActiveOrders = await orderRepository.GetUserViewConfirmedActiveOrdersAsync();
            return View("ShoppingBag", await shoppingBagRepository.GetUserActiveShoppingBagItemsAsync());
        }

        [AllowAnonymous]
        public async Task<bool> InsertUsingProductAsync(int productId)
        {
            if (User.Identity?.IsAuthenticated == true && User.IsInRole("User"))
                return await shoppingBagRepository.InsertUsingProductAsync(productId);
            else
                return false;
        }

        [HttpPost]
        public async Task<bool> UpdateUsingJsonObjectsArrayAsync(string itemsArray)
        {
            return await shoppingBagRepository.UpdateUsingJsonObjectsArrayAsync(itemsArray);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await shoppingBagRepository.GetByIdAsync(id);
            return await shoppingBagRepository.DeleteAsync(entity!);
        }
    }
}
