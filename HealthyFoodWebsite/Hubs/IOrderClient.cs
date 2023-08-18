using HealthyFoodWebsite.Hubs.Dtos;

namespace HealthyFoodWebsite.Hubs
{
    public interface IOrderClient
    {
        Task SendOrderIdAsync(int orderId);

        Task SendSpecificOrderIdAsync(int orderId);

        Task SendOrderAsync(OrderDto currentOrder);

        Task SendOrderToUserAsync(OrderDto currentOrder);

        Task SendOrderIdWithItsModeToUserAsync(int id, string mode);

        Task SendOrderIdWithItsInactiveStatusToUserAsync(int id, string orderStatus);
    }
}
