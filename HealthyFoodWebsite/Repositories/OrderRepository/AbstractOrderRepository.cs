using HealthyFoodWebsite.Models;

namespace HealthyFoodWebsite.Repositories.OrderRepository
{
    public abstract class AbstractOrderRepository : IRepository<Order>
    {
        public Task<List<Order>> GetAllAsync() => throw new NotImplementedException();

        public abstract Task<Order?> GetByIdAsync(int id);

        public Task<bool> InsertAsync(Order entity) => throw new NotImplementedException();

        public abstract Task<bool> UpdateAsync(Order entity);

        public Task<bool> DeleteAsync(Order entity) => throw new NotImplementedException();


        // Child Object Methods Zone
        public abstract Task<List<Order>> GetAdminViewActiveOrdersAsync();

        public abstract Task<List<Order>> GetAdminViewInactiveOrdersAsync();

        public abstract Task<List<Order>> GetUserViewConfirmedOrdersAsync();

        public abstract Task<List<Order>> GetUserViewConfirmedActiveOrdersAsync();

        public abstract Task<int> InsertThenReturnIdAsync(Order entity);

        public abstract Task<bool> ChangePreparingOrDeliveringToTrue(Order entity, string mode);

        public abstract Task<bool> SealOrderAsDoneOrCancelled(Order entity, string status);

        public abstract Task<bool> PerformUserOrAdminViewDeletionAsync(Order entity, string view);
    }
}
