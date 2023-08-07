using HealthyFoodWebsite.Models;

namespace HealthyFoodWebsite.Repositories.ContactUsRepository
{
    public abstract class AbstractContactUsRepository : IRepository<CustomerMessage>
    {
        public abstract Task<List<CustomerMessage>> GetAllAsync();

        public abstract Task<CustomerMessage?> GetByIdAsync(int id);

        public abstract Task<bool> InsertAsync(CustomerMessage entity);

        public Task<bool> UpdateAsync(CustomerMessage entity) => throw new NotImplementedException();

        public abstract Task<bool> DeleteAsync(CustomerMessage entity);
    }
}
