using HealthyFoodWebsite.Models;

namespace HealthyFoodWebsite.Repositories.ShoppingBag
{
    public abstract class AbstractShoppingBagRepository : IRepository<ShoppingBagItem>
    {
        public Task<List<ShoppingBagItem>> GetAllAsync() => throw new NotImplementedException();

        public abstract Task<ShoppingBagItem?> GetByIdAsync(int id);

        public Task<bool> InsertAsync(ShoppingBagItem entity) => throw new NotImplementedException();

        public Task<bool> UpdateAsync(ShoppingBagItem entity) => throw new NotImplementedException();

        public abstract Task<bool> DeleteAsync(ShoppingBagItem entity);


        // Child Object Methods Zone
        public abstract Task<List<ShoppingBagItem>> GetUserShoppingBagItemsAsync();

        public abstract Task<bool> InsertUsingProductAsync(int productId);

        public abstract Task<bool> UpdateUsingJsonObjectsArrayAsync(string itemsArray);
    }
}
