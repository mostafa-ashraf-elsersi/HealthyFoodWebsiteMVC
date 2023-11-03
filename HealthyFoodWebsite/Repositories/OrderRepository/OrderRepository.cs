using HealthyFoodWebsite.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HealthyFoodWebsite.Repositories.OrderRepository
{
    public class OrderRepository : AbstractOrderRepository
    {
        // Object Fields Zone
        private readonly HealthyFoodDbContext dbContext;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly SemaphoreSlim semaphoreSlim = new(1, 1);


        // Dependency Injection Zone
        public OrderRepository(HealthyFoodDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            this.dbContext = dbContext;
            this.httpContextAccessor = httpContextAccessor;
        }


        // Object Methods Zone
        public override async Task<List<Order>> GetAdminViewActiveOrdersAsync()
        {
            await semaphoreSlim.WaitAsync(-1);

            var activeOrders = await dbContext
               .Order
               .Where(order => order.Status == "Active" && order.AdminIsDeleted == false && order.UserIsDeleted == false)
               .Include(order => order.ShoppingBagItems)
               .Include(order => order.Logger)
               .OrderBy(order => order.InitiatingDateAndTime)
               .AsNoTracking()
               .ToListAsync();

            semaphoreSlim.Release();

            return activeOrders;
        }

        public override async Task<List<Order>> GetAdminViewInactiveOrdersAsync()
        {
            await semaphoreSlim.WaitAsync(-1);

            var inactiveOrders = await dbContext
               .Order
               .Where(order => order.Status != "Active" && order.AdminIsDeleted == false && order.UserIsDeleted == false)
               .Include(order => order.ShoppingBagItems)
               .Include(order => order.Logger)
               .OrderBy(order => order.InitiatingDateAndTime)
               .AsNoTracking()
               .ToListAsync();

            semaphoreSlim.Release();

            return inactiveOrders;
        }

        public override async Task<List<Order>> GetUserViewConfirmedOrdersAsync()
        {
            await semaphoreSlim.WaitAsync(-1);

            var confirmedOrders = await dbContext
              .Order
              .Where(order => order.LoggerId.ToString() == httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.SerialNumber) && order.UserIsDeleted == false)
              .Include(order => order.ShoppingBagItems!
                .Where(item => item.Status == "Confirmed"))
              .AsNoTracking()
              .ToListAsync();

            semaphoreSlim.Release();

            return confirmedOrders;
        }

        public override async Task<List<Order>> GetUserViewConfirmedActiveOrdersAsync()
        {
            await semaphoreSlim.WaitAsync(-1);

            var activeOrders = await dbContext
              .Order
              .Where(order => order.LoggerId.ToString() == httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.SerialNumber) && order.Status == "Active" && order.UserIsDeleted == false)
              .Include(order => order.ShoppingBagItems!
                .Where(item => item.Status == "Confirmed"))
              .AsNoTracking()
              .ToListAsync();

            semaphoreSlim.Release();

            return activeOrders;
        }

        public override async Task<Order?> GetByIdAsync(int id)
        {
            await semaphoreSlim.WaitAsync(-1);

            var order = await dbContext
               .Order
               .Where(order => order.Id == id)
               .Include(order => order.ShoppingBagItems)
               .Include(order => order.Logger)
               .FirstOrDefaultAsync();

            semaphoreSlim.Release();

            return order;
        }

        public override async Task<int> InsertThenReturnIdAsync(Order entity)
        {
            try
            {
                await semaphoreSlim.WaitAsync(-1);

                await dbContext.Order.AddAsync(entity);
                await dbContext.SaveChangesAsync();

                semaphoreSlim.Release();

                return entity.Id;
            }
            catch
            {
                return entity.Id;
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

        public override async Task<bool> ChangePreparingOrDeliveringToTrue(Order entity, string mode)
        {
            try
            {
                if (mode == "Preparing")
                    entity.StartedPreparing = true;
                else if (mode == "Delivering")
                    entity.StartedDelivering = true;

                await semaphoreSlim.WaitAsync(-1);

                await dbContext.SaveChangesAsync();

                semaphoreSlim.Release();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public override async Task<bool> SealOrderAsDoneOrCancelled(Order entity, string status)
        {
            try
            {
                if (status == "Done")
                    entity.Status = "Done";
                else if (status == "Cancelled")
                    entity.Status = "Cancelled";

                await semaphoreSlim.WaitAsync(-1);

                await dbContext.SaveChangesAsync();

                semaphoreSlim.Release();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public override async Task<bool> PerformUserOrAdminViewDeletionAsync(Order entity, string view)
        {
            try
            {
                if (view == "AdminView")
                    entity.AdminIsDeleted = true;
                else if (view == "UserView")
                {
                    if (entity.Status == "Active")
                        entity.Status = "CancelledFromUser";
                    entity.UserIsDeleted = true;
                }
                    

                await semaphoreSlim.WaitAsync(-1);

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
