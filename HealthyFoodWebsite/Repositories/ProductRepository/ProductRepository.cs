using HealthyFoodWebsite.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthyFoodWebsite.Repositories.ProductRepository
{
    public class ProductRepository : AbstractProductRepository
    {
        // Object Fields Zone
        private readonly HealthyFoodDbContext dbContext;
        private readonly ImageUploader.ImageUploader imageUploader;
        private readonly SemaphoreSlim semaphoreSlim = new(1, 1);


        // Dependency Injection Zone
        public ProductRepository(HealthyFoodDbContext dbContext, ImageUploader.ImageUploader imageUploader)
        {
            this.dbContext = dbContext;
            this.imageUploader = imageUploader;
        }


        // Object Methods Zone
        public override async Task<List<Product>> GetAllAsync()
        {
            await semaphoreSlim.WaitAsync(-1);

            var products = await dbContext.Product.AsNoTracking().ToListAsync();

            semaphoreSlim.Release();

            return products;
        }

        public override async Task<List<Product>> FilterByCategoryAsync(string category)
        {
            if (category == "All")
                return await GetAllAsync();
            
            await semaphoreSlim.WaitAsync(-1);

            var products = await dbContext
                .Product
                .Where(product => product.Category == category)
                .AsNoTracking()
                .ToListAsync();

            semaphoreSlim.Release();

            return products;
        }

        public override async Task<Product?> GetByIdAsync(int id)
        {
            await semaphoreSlim.WaitAsync(-1);

            var product = await dbContext.Product.FindAsync(id);

            semaphoreSlim.Release();

            return product;
        }

        public override async Task<bool> InsertAsync(Product entity)
        {
            try
            {
                var imageUri = await imageUploader.UploadImageToServerAsync(entity.ImageFile, "\\img\\products\\");
                entity.ImageUri = imageUri;

                await semaphoreSlim.WaitAsync(-1);

                await dbContext.Product.AddAsync(entity);
                await dbContext.SaveChangesAsync();
            
                semaphoreSlim.Release();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public override async Task<bool> UpdateAsync(Product entity)
        {
            try
            {
                if (entity.ImageFile is not null)
                {
                    if (!imageUploader.DeleteImageFromServerAsync(entity.ImageUri))
                        throw new Exception("An error occurred in the deletion function of the image uploader service.");

                    var imageUri = await imageUploader.UploadImageToServerAsync(entity.ImageFile, "\\img\\products\\");
                    entity.ImageUri = imageUri;
                }

                await semaphoreSlim.WaitAsync(-1);

                dbContext.Product.Update(entity);
                await dbContext.SaveChangesAsync();

                semaphoreSlim.Release();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public override async Task<bool> DeactivateAsync(Product entity)
        {
            try
            {
                if (entity.IsDisplayed == true)
                    entity.IsDisplayed = false;

                else if (entity.IsDisplayed == false)
                    entity.IsDisplayed = true;

                else
                    throw new Exception();

                await semaphoreSlim.WaitAsync(-1);

                await dbContext.SaveChangesAsync();

                semaphoreSlim.Release();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public override async Task<bool> DeleteAsync(Product entity)
        {
            try
            {
                if (!imageUploader.DeleteImageFromServerAsync(entity.ImageUri))
                    throw new Exception("An error occurred in the deletion function of the image uploader service.");

                await semaphoreSlim.WaitAsync(-1);

                dbContext.Product.Remove(entity);
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
        ~ProductRepository()
        {
            semaphoreSlim.Dispose();
        }
    }
}
