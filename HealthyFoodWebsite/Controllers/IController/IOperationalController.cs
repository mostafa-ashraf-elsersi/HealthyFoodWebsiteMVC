using Microsoft.AspNetCore.Mvc;

namespace HealthyFoodWebsite.Controllers.IController
{
    public interface IOperationalController<T>
    {
        // All Methods Signatures Below, Are Optional
        Task<IActionResult> GetAllAsync() => throw new NotImplementedException();

        Task<T?> GetByIdAsync(int id) => throw new NotImplementedException();

        Task<bool> InsertAsync(T entity) => throw new NotImplementedException();

        Task<bool> UpdateAsync(T entity) => throw new NotImplementedException();

        Task<bool> DeleteAsync(T entity) => throw new NotImplementedException();
    }
}
