using HealthyFoodWebsite.Hubs.Dtos;

namespace HealthyFoodWebsite.Hubs
{
    public interface IOrderClient
    {
        Task SendOrderAsync(OrderDto currentOrder);
    }
}
