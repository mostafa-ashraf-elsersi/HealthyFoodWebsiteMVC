﻿using HealthyFoodWebsite.Models;

namespace HealthyFoodWebsite.Repositories.LoggerRepository
{
    public abstract class AbstractLoggerRepository : IRepository<Logger>
    {
        public Task<List<Logger>> GetAllAsync() => throw new NotImplementedException();
        
        public abstract Task<Logger?> GetByIdAsync(int id);
        
        public abstract Task<bool> InsertAsync(Logger entity);
        
        public abstract Task<bool> UpdateAsync(Logger entity);
        
        public abstract Task<bool> DeleteAsync(Logger entity);


        // Child Object Methods Zone
        public abstract Task<List<Logger>> GetAllAdminsAsync();
        
        public abstract Task<bool> DeactivateAsync(Logger entity);
        
        public abstract Task<Logger?> CheckSystemLoggerExistence(string username, string password);
    }
}
