using HealthyFoodWebsite.Models;
using HealthyFoodWebsite.Repositories.ProductRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace HealthyFoodWebsite.Repositories.ShoppingBag
{
    public class ShoppingBagRepository : AbstractShoppingBagRepository
    {
        // Object Fields Zone
        private readonly HealthyFoodDbContext dbContext;
        private readonly AbstractProductRepository productRepository;
        private readonly SemaphoreSlim semaphoreSlim = new(1, 1);


        // Dependency Injection Zone
        public ShoppingBagRepository(HealthyFoodDbContext dbContext, AbstractProductRepository productRepository)
        {
            this.dbContext = dbContext;
            this.productRepository = productRepository;
        }


        // Object Methods Zone
        public override async Task<List<ShoppingBagItem>> GetUserShoppingBagItemsAsync()
        {
            await semaphoreSlim.WaitAsync(-1);

            var userItems = await dbContext
               .ShoppingBag
               .Where(item => item.LoggerId == 1 && item.Status == "Active") // TODO: Get the current logger Id.
               .AsNoTracking()
               .ToListAsync();

            semaphoreSlim.Release();

            return userItems;
        }

        public override async Task<ShoppingBagItem?> GetByIdAsync(int id)
        {
            await semaphoreSlim.WaitAsync(-1);

            var item = await dbContext
               .ShoppingBag
               .FindAsync(id);

            semaphoreSlim.Release();

            return item;
        }

        public override async Task<bool> InsertUsingProductAsync(int productId)
        {
            try
            {
                var product = await productRepository.GetByIdAsync(productId);

                var shoppingBagItem = new ShoppingBagItem
                {
                    Name = product!.Name,
                    UnitPrice = product.Price,
                    SubTotalPrice = product.Price,
                    LoggerId = 1 // TODO: Get the current logger Id.
                };

                await semaphoreSlim.WaitAsync(-1);

                await dbContext
                    .ShoppingBag
                    .AddAsync(shoppingBagItem);
                await dbContext.SaveChangesAsync();

                semaphoreSlim.Release();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public override async Task<bool> UpdateUsingJsonObjectsArrayAsync(string itemsArray)
        {
            try
            {
                var entitiesList = JsonConvert.DeserializeObject<List<ShoppingBagItem>>(itemsArray);

                if (entitiesList.IsNullOrEmpty())
                    return false;

                await semaphoreSlim.WaitAsync(-1);

                dbContext.ShoppingBag.UpdateRange(entitiesList!);
                await dbContext.SaveChangesAsync();

                semaphoreSlim.Release();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public override async Task<bool> DeleteAsync(ShoppingBagItem entity)
        {
            try
            {
                await semaphoreSlim.WaitAsync(-1);

                dbContext.ShoppingBag.Remove(entity);
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
        ~ShoppingBagRepository()
        {
            semaphoreSlim.Dispose();
        }
    }
}
