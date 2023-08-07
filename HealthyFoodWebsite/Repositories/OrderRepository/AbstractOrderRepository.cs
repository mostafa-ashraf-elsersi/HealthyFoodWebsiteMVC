using HealthyFoodWebsite.Models;

namespace HealthyFoodWebsite.Repositories.OrderRepository
{
    public abstract class AbstractOrderRepository : IRepository<Order>
    {
        public Task<List<Order>> GetAllAsync() => throw new NotImplementedException();

        public abstract Task<Order?> GetByIdAsync(int id);

        public abstract Task<bool> InsertAsync(Order entity);

        public abstract Task<bool> UpdateAsync(Order entity);

        public abstract Task<bool> DeleteAsync(Order entity);


        // Child Object Methods Zone
        public abstract Task<List<Order>> GetActiveOrdersAsync();

        public abstract Task<List<Order>> GetInactiveOrdersAsync();

        public abstract Task<int?> GetLastInsertedOrderId();
    }
}
