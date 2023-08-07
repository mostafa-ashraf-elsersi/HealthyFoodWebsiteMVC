using HealthyFoodWebsite.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthyFoodWebsite.Repositories.LoggerRepository
{
    public class LoggerRepository : AbstractLoggerRepository
    {
        // Object Fields Zone
        private readonly HealthyFoodDbContext dbContext;
        private readonly SemaphoreSlim semaphoreSlim = new(1, 1);


        // Dependency Injection Zone
        public LoggerRepository(HealthyFoodDbContext dbContext) =>
            this.dbContext = dbContext;


        // Object Methods Zone
        public override async Task<List<Logger>> GetAllAdminsAsync()
        {
            await semaphoreSlim.WaitAsync(-1);

            var admins = await dbContext.Logger.AsNoTracking().ToListAsync();

            semaphoreSlim.Release();

            return admins;
        }

        public override async Task<Logger?> GetByIdAsync(int id)
        {
            await semaphoreSlim.WaitAsync(-1);

            var logger = await dbContext.Logger.FindAsync(id);

            semaphoreSlim.Release();

            return logger;
        }

        public override async Task<Logger?> CheckSystemLoggerExistence(string username, string password)
        {
            await semaphoreSlim.WaitAsync(-1);

            var logger = await dbContext
                .Logger
                .Where(logger => logger.Username == username && logger.Password == password)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            semaphoreSlim.Release();

            return logger;
        }

        public override async Task<bool> InsertAsync(Logger entity)
        {
            try
            {
                await semaphoreSlim.WaitAsync(-1);

                await dbContext.Logger.AddAsync(entity);
                await dbContext.SaveChangesAsync();

                semaphoreSlim.Release();

                return true;
            }
            catch
            {
                return false;
            }

        }

        public override async Task<bool> UpdateAsync(Logger entity)
        {
            try
            {
                await semaphoreSlim.WaitAsync(-1);

                dbContext.Logger.Update(entity);
                await dbContext.SaveChangesAsync();

                semaphoreSlim.Release();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public override async Task<bool> DeactivateAsync(Logger entity)
        {
            try
            {
                entity.IsActive = false;

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

        public override async Task<bool> DeleteAsync(Logger entity)
        {
            try
            {
                await semaphoreSlim.WaitAsync(-1);

                dbContext.Logger.Remove(entity);
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
        ~LoggerRepository() => semaphoreSlim.Dispose();
    }
}

