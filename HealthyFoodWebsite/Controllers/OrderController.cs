using HealthyFoodWebsite.Models;
using HealthyFoodWebsite.Repositories.OrderRepository;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "BusinessOwner, Admin")]
        public async Task<IActionResult> GetUsersOrdersAsync()
        {
            ViewBag.UsersInactiveOrders = await orderRepository.GetAdminViewInactiveOrdersAsync();
            return View("Order", await orderRepository.GetAdminViewActiveOrdersAsync());
        }

        [Authorize(Roles = "BusinessOwner, Admin")]
        public async Task<bool> ChangePreparingOrDeliveringToTrue(int id, string mode)
        {
            var entity = await orderRepository.GetByIdAsync(id);
            return await orderRepository.ChangePreparingOrDeliveringToTrue(entity!, mode);
        }

        [Authorize(Roles = "BusinessOwner, Admin")]
        public async Task<bool> SealOrderAsDoneOrCancelled(int id, string status)
        {
            var entity = await orderRepository.GetByIdAsync(id);
            return await orderRepository.SealOrderAsDoneOrCancelled(entity!, status);
        }

        [Authorize(Roles = "BusinessOwner, Admin, User")]
        public async Task<bool> PerformUserOrAdminViewDeletionAsync(int id, string view)
        {
            var entity = await orderRepository.GetByIdAsync(id);
            return await orderRepository.PerformUserOrAdminViewDeletionAsync(entity!, view);
        }
    }
}
