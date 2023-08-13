using HealthyFoodWebsite.Hubs.Dtos;

namespace HealthyFoodWebsite.Hubs
{
    public interface IOrderClient
    {
        Task SendOrderIdAsync(int orderId);

        Task SendOrderAsync(OrderDto currentOrder);

        Task SendOrderToUserAsync(OrderDto currentOrder);
    }
}
