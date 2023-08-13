using HealthyFoodWebsite.Models;
using HealthyFoodWebsite.Repositories.OrderRepository;
using HealthyFoodWebsite.Hubs.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using HealthyFoodWebsite.Repositories.ShoppingBag;

namespace HealthyFoodWebsite.Hubs
{
    public class OrderHub : Hub<IOrderClient>
    {

        public async Task PersistOrderInDatabaseThenReturnId(string orderDetails, [FromServices] AbstractOrderRepository orderRepository)
        {
            try
            {
                var order = JsonConvert.DeserializeObject<Order>(orderDetails);

                var orderId = await orderRepository.InsertThenReturnIdAsync(order!);

                await Clients.Caller.SendOrderIdAsync(orderId);
            }
            catch
            {
                return;
            }
          
        }

        public async Task RedirectOrderFromCustomerToSeller(int orderId, [FromServices] AbstractOrderRepository orderRepository)
        {
            try
            {
                List<ShoppingBagItemDto> shoppingBagItems = new();

                var order = await orderRepository.GetByIdAsync(orderId);

                var items = order!.ShoppingBagItems.ToList();

                items.ForEach(item =>
                {
                    shoppingBagItems.Add(new ShoppingBagItemDto(
                        item.Name,
                        item.UnitPrice,
                        item.Quantity,
                        item.SubTotalPrice));
                });

                var currentOrder = new OrderDto(
                    order.Id,
                    order.Logger?.FullName,
                    order.Logger?.PhoneNumber,
                    order.InitiatingDateAndTime,
                    order.Status,
                    order.TotalCost,
                    order.StartedPreparing,
                    order.StartedDelivering,
                    shoppingBagItems);

                await Clients.Others.SendOrderAsync(currentOrder);
                await Clients.Caller.SendOrderToUserAsync(currentOrder);
            }
            catch
            {
                return;
            }
          
        }
    }
}
