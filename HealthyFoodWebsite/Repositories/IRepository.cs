namespace HealthyFoodWebsite.Repositories
{
    public interface IRepository<T>
    {
        Task<List<T>> GetAllAsync();

        Task<T?> GetByIdAsync(int id);

        Task<bool> InsertAsync(T entity);

        Task<bool> UpdateAsync(T entity);

        Task<bool> DeleteAsync(T entity);
    }
}
