using HealthyFoodWebsite.Models;
using HealthyFoodWebsite.Repositories.OrderRepository;
using Microsoft.AspNetCore.Mvc;

namespace HealthyFoodWebsite.Controllers
{
    public class OrderController : Controller
    {
        // Object Fields Zone
        private readonly AbstractOrderRepository orderRepository;


        // Dependency Injection Zone
        public OrderController(AbstractOrderRepository orderRepository) =>
            this.orderRepository = orderRepository;


        // Object Methods Zone
        public async Task<IActionResult> GetUserOrdersAsync()
        {
            ViewBag.UsersInactiveOrders = await orderRepository.GetAdminViewInactiveOrdersAsync();
            return View("Order", await orderRepository.GetAdminViewActiveOrdersAsync());
        }

        public async Task<bool> UpdateAsync(Order entity)
        {
            return await orderRepository.UpdateAsync(entity);
        }

        public async Task<bool> ChangePreparingOrDeliveringToTrue(int id, string mode)
        {
            var entity = await orderRepository.GetByIdAsync(id);
            return await orderRepository.ChangePreparingOrDeliveringToTrue(entity!, mode);
        }

        public async Task<bool> SealOrderAsDoneOrCancelled(int id, string status)
        {
            var entity = await orderRepository.GetByIdAsync(id);
            return await orderRepository.SealOrderAsDoneOrCancelled(entity!, status);
        }

        public async Task<bool> PerformUserOrAdminViewDeletionAsync(int id, string view)
        {
            var entity = await orderRepository.GetByIdAsync(id);
            return await orderRepository.PerformUserOrAdminViewDeletionAsync(entity!, view);
        }
    }
}
