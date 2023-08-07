using HealthyFoodWebsite.Models;
using HealthyFoodWebsite.Repositories.OrderRepository;
using HealthyFoodWebsite.Hubs.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace HealthyFoodWebsite.Hubs
{
    public class OrderHub : Hub<IOrderClient>
    {
        public async Task PersistOrderInDatabaseThenRedirect(string orderDetails, [FromServices] AbstractOrderRepository orderRepository)
        {
            try
            {
                var order = JsonConvert.DeserializeObject<Order>(orderDetails);

                await orderRepository.InsertAsync(order!);

                var lastInsertedOrderId = await orderRepository.GetLastInsertedOrderId();

                var _order = await orderRepository.GetByIdAsync((int)lastInsertedOrderId);

                await RedirectOrderFromCustomerToSeller(_order);
            }
            catch
            {
                return;
            }
          
        }

        public async Task RedirectOrderFromCustomerToSeller(Order? order)
        {
            List<ShoppingBagItemDto> shoppingBagItems = new();

            order!.Logger?.ShoppingBag.ToList().ForEach(item =>
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
                order.StartedPreparing,
                order.StartedDelivering,
                shoppingBagItems);

            await Clients.Others.SendOrderAsync(currentOrder);
        }
    }
}
