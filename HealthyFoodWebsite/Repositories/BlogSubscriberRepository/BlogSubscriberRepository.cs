using HealthyFoodWebsite.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace HealthyFoodWebsite.Repositories.BlogSubscriberRepository
{
    public class BlogSubscriberRepository : AbstractBlogSubscriberRepository
    {
        // Object Fields Zone
        private readonly HealthyFoodDbContext dbContext;
        private readonly SemaphoreSlim semaphoreSlim = new(1, 1);


        // Dependency Injection Zone
        public BlogSubscriberRepository(HealthyFoodDbContext dbContext) =>
            this.dbContext = dbContext;


        // Object Methods Zone
        public override async Task<List<BlogSubscriber>> GetAllAsync()
        {
            await semaphoreSlim.WaitAsync(-1);

            var subscribers = await dbContext.BlogSubscriber.AsNoTracking().ToListAsync();

            semaphoreSlim.Release();

            return subscribers;
        }

        public override async Task<bool> SearchForSubscriptionsOfUsername(string username)
        {
            try
            {
                await semaphoreSlim.WaitAsync(-1);

                var subscriptions = await dbContext
                    .BlogSubscriber
                    .Where(subscriber => subscriber.UserName == username)
                    .AsNoTracking()
                    .ToListAsync();

                semaphoreSlim.Release();

                return !subscriptions.IsNullOrEmpty();
            }
            catch
            {
                return false;
            }
        }

        public override async Task<List<BlogSubscriber>> GetUserSubscriptionsAsync(string username)
        {
            await semaphoreSlim.WaitAsync(-1);

            var subscriptions = await dbContext
                .BlogSubscriber
                .Where(subscriber => subscriber.UserName == username)
                .AsNoTracking()
                .ToListAsync();

            semaphoreSlim.Release();

            return subscriptions;
        }

        public override async Task<BlogSubscriber?> GetByIdAsync(int id)
        {
            await semaphoreSlim.WaitAsync(-1);

            var subscriber = await dbContext.BlogSubscriber.FindAsync(id);

            semaphoreSlim.Release();

            return subscriber;
        }

        public override async Task<bool> InsertAsync(BlogSubscriber entity)
        {
            try
            {
                await semaphoreSlim.WaitAsync(-1);

                await dbContext.BlogSubscriber.AddAsync(entity);
                await dbContext.SaveChangesAsync();

                semaphoreSlim.Release();

                return true;
            }
            catch
            {
                return false;
            }

        }

        public override async Task<bool> DeleteSubscriptionsAsync(List<BlogSubscriber> entities)
        {
            try
            {
                await semaphoreSlim.WaitAsync(-1);

                dbContext.BlogSubscriber.RemoveRange(entities);
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
        ~BlogSubscriberRepository()
        {
            semaphoreSlim.Dispose();
        }
    }
}
