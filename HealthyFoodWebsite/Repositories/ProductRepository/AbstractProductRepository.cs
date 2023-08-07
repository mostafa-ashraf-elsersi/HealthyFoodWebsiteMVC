using HealthyFoodWebsite.Models;

namespace HealthyFoodWebsite.Repositories.ProductRepository
{
    public abstract class AbstractProductRepository : IRepository<Product>
    {
        public abstract Task<List<Product>> GetAllAsync();

        public abstract Task<Product?> GetByIdAsync(int id);

        public abstract Task<bool> InsertAsync(Product entity);

        public abstract Task<bool> UpdateAsync(Product entity);

        public abstract Task<bool> DeleteAsync(Product entity);


        // Child Object Methods Zone
        public abstract Task<List<Product>> FilterByCategoryAsync(string category);

        public abstract Task<bool> DeactivateAsync(Product entity);
    }
}
