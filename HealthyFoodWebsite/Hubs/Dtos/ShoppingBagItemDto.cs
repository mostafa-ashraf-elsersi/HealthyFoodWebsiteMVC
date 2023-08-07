namespace HealthyFoodWebsite.Hubs.Dtos
{
    public sealed record ShoppingBagItemDto(
        string ProductName,
        float Price,
        int Quantity,
        float SubTotal);
}
