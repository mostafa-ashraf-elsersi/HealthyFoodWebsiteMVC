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
        public async Task<IActionResult> GetActiveOrdersAsync()
        {
            return View("Order", await orderRepository.GetAdminViewActiveOrdersAsync());
        }

        public async Task<IActionResult> GetInactiveOrdersAsync()
        {
            return View(await orderRepository.GetAdminViewInactiveOrdersAsync());
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await orderRepository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateAsync(Order entity)
        {
            return await orderRepository.UpdateAsync(entity);
        }

        public async Task<bool> DeleteAsync(Order entity)
        {
            return await orderRepository.DeleteAsync(entity);
        }
    }
}
