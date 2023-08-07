using HealthyFoodWebsite.Models;

namespace HealthyFoodWebsite.Repositories.BlogRepository
{
    public abstract class AbstractBlogPostRepository : IRepository<BlogPost>
    {
        public Task<List<BlogPost>> GetAllAsync() => throw new NotImplementedException();

        public abstract Task<BlogPost?> GetByIdAsync(int id);

        public abstract Task<bool> InsertAsync(BlogPost entity);

        public abstract Task<bool> UpdateAsync(BlogPost entity);

        public abstract Task<bool> DeleteAsync(BlogPost entity);


        // Child Object Methods 
        public abstract Task<(List<BlogPost>, int)> GetPagesWithCountAsync(int pageIndex);

        public abstract Task<List<BlogPost>> GetLastThreePostsAsync();

        public abstract Task<bool> DeactivateAsync(BlogPost entity);
    }
}
