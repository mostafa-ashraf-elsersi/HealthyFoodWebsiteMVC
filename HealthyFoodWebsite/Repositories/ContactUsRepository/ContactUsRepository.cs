using HealthyFoodWebsite.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthyFoodWebsite.Repositories.ContactUsRepository
{
    public class ContactUsRepository : AbstractContactUsRepository
    {
        // Object Fields Zone
        private readonly HealthyFoodDbContext dbContext;
        private readonly SemaphoreSlim semaphoreSlim = new(1, 1);


        // Dependency Injection Zone
        public ContactUsRepository(HealthyFoodDbContext dbContext) =>
            this.dbContext = dbContext;


        // Object Methods Zone
        public override async Task<List<CustomerMessage>> GetAllAsync()
        {
            await semaphoreSlim.WaitAsync(-1);

            var messages = await dbContext.CustomerMessage.AsNoTracking().ToListAsync();

            semaphoreSlim.Release();

            return messages;
        }

        public override async Task<CustomerMessage?> GetByIdAsync(int id)
        {
            await semaphoreSlim.WaitAsync(-1);

            var message = await dbContext.CustomerMessage.FindAsync(id);

            semaphoreSlim.Release();

            return message;
        }

        public override async Task<bool> InsertAsync(CustomerMessage entity)
        {
            try
            {
                await semaphoreSlim.WaitAsync(-1);

                await dbContext.CustomerMessage.AddAsync(entity);
                await dbContext.SaveChangesAsync();

                semaphoreSlim.Release();

                return true;
            }
            catch
            {
                return false;
            }

        }

        public override async Task<bool> DeleteAsync(CustomerMessage entity)
        {
            try
            {
                await semaphoreSlim.WaitAsync(-1);

                dbContext.CustomerMessage.Remove(entity);
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
        ~ContactUsRepository()
        {
            semaphoreSlim.Dispose();
        }
    }
}
