namespace HealthyFoodWebsite.Hubs.Dtos
{
    public sealed record OrderDto(
        int OrderId,
        string? CustomerName,
        string? PhoneNumber,
        DateTime InitiatingDateAndTime,
        string Status,
        bool StartedPreparing,
        bool StartedDelivering,
        List<ShoppingBagItemDto> ShoppingBagItems);
}
