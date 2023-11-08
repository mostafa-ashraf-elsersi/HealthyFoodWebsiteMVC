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
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> InsertUsingProductAsync(int productId)
        {
            var productInsertionResult = 0;

            if (User.Identity?.IsAuthenticated == true && User.IsInRole("User"))
            {
                if (await shoppingBagRepository.CheckProductExsitenceInShoppingBagAsync(productId))
                {
                    productInsertionResult = 2;
                }
                else
                {
                    productInsertionResult = await shoppingBagRepository.InsertUsingProductAsync(productId) ? 1 : 0;
                }
            }
           
            return RedirectToActionPermanentPreserveMethod("GetAll", "Product", new { ProductInsertionResult = productInsertionResult });
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
