using HealthyFoodWebsite.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthyFoodWebsite.Repositories.OrderRepository
{
    public class OrderRepository : AbstractOrderRepository
    {
        // Object Fields Zone
        private readonly HealthyFoodDbContext dbContext;
        private readonly SemaphoreSlim semaphoreSlim = new(1, 1);


        // Dependency Injection Zone
        public OrderRepository(HealthyFoodDbContext dbContext) =>
            this.dbContext = dbContext;


        // Object Methods Zone
        public override async Task<List<Order>> GetActiveOrdersAsync()
        {
            await semaphoreSlim.WaitAsync(-1);

            var activeOrders = await dbContext
               .Order
               .Where(order => order.Status == "Active")
               .Include(order => order.Logger)
               .ThenInclude(logger => logger!.ShoppingBag)
               .OrderBy(order => order.InitiatingDateAndTime)
               .AsNoTracking()
               .ToListAsync();

            semaphoreSlim.Release();

            return activeOrders;
        }

        public override async Task<List<Order>> GetInactiveOrdersAsync()
        {
            await semaphoreSlim.WaitAsync(-1);

            var inactiveOrders = await dbContext
               .Order
               .Where(order => order.Status != "Active")
               .Include(order => order.Logger)
               .ThenInclude(logger => logger!.ShoppingBag)
               .OrderBy(order => order.InitiatingDateAndTime)
               .AsNoTracking()
               .ToListAsync();

            semaphoreSlim.Release();

            return inactiveOrders;
        }

        public override async Task<int?> GetLastInsertedOrderId()
        {
            await semaphoreSlim.WaitAsync(-1);

            var lastInsertedOrder = await dbContext
                .Order
                .OrderBy(order => order.Id)
                .AsNoTracking()
                .LastOrDefaultAsync();

            semaphoreSlim.Release();

            return lastInsertedOrder?.Id;
        }

        public override async Task<Order?> GetByIdAsync(int id)
        {
            await semaphoreSlim.WaitAsync(-1);

            var order = await dbContext
               .Order
               .Where(order => order.Status == "Active" && order.Id == id)
               .Include(order => order.Logger)
               .ThenInclude(logger => logger!.ShoppingBag)
               .FirstOrDefaultAsync();

            semaphoreSlim.Release();

            return order;
        }

        public override async Task<bool> InsertAsync(Order entity)
        {
            try
            {
                await semaphoreSlim.WaitAsync(-1);

                await dbContext.Order.AddAsync(entity);
                await dbContext.SaveChangesAsync();

                semaphoreSlim.Release();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public override async Task<bool> UpdateAsync(Order entity)
        {
            try
            {
                await semaphoreSlim.WaitAsync(-1);

                dbContext.Order.Update(entity);
                await dbContext.SaveChangesAsync();

                semaphoreSlim.Release();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public override async Task<bool> DeleteAsync(Order entity)
        {
            try
            {
                await semaphoreSlim.WaitAsync(-1);

                dbContext.Order.Remove(entity);
                await dbContext.SaveChangesAsync();

                semaphoreSlim.Release();

                return true;
            }
            catch
            {
                return false;
            }
        }


        // Disposing Objects Zone
        ~OrderRepository() => semaphoreSlim.Dispose();
    }
}
