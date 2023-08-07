using HealthyFoodWebsite.Repositories;
using HealthyFoodWebsite.Models;

namespace HealthyFoodWebsite.Repositories.BlogSubscriberRepository
{
    public abstract class AbstractBlogSubscriberRepository : IRepository<BlogSubscriber>
    {
        public abstract Task<List<BlogSubscriber>> GetAllAsync();

        public abstract Task<BlogSubscriber?> GetByIdAsync(int id);

        public abstract Task<bool> InsertAsync(BlogSubscriber entity);

        public Task<bool> UpdateAsync(BlogSubscriber entity) => throw new NotImplementedException();

        public abstract Task<bool> DeleteAsync(BlogSubscriber entity);
    }
}
