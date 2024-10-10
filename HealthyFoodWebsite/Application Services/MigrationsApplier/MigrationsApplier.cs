using HealthyFoodWebsite.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace HealthyFoodWebsite.Application_Services.MigrationsApplier
{
    public static class MigrationsApplier
    {
        public static void ApplyMigrationsIfNot(this WebApplication? webApplication)
        {
            using var scope = webApplication?.Services.CreateScope();

            var dbContext = scope?.ServiceProvider.GetService<HealthyFoodDbContext>();

            if (dbContext is not null)
            {
                dbContext.Database.Migrate();
            }
        }
    }
}
