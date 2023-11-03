using HealthyFoodWebsite.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HealthyFoodWebsite.Repositories.TestimonialRepository
{
    public class TestimonialRepository : AbstractTestimonialRepository
    {
        // Object Fields Zone
        private readonly HealthyFoodDbContext dbContext;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly SemaphoreSlim semaphoreSlim = new(1, 1);


        // Dependency Injection Zone
        public TestimonialRepository(HealthyFoodDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            this.dbContext = dbContext;
            this.httpContextAccessor = httpContextAccessor;
        }

        // Object Methods Zone
        public override async Task<List<Testimonial>> GetAllAsync()
        {
            await semaphoreSlim.WaitAsync(-1);

            var testimonials = await dbContext
                .Testimonial
                .Include(testimonial => testimonial.Logger)
                .OrderBy(testimonial => testimonial.SeenByAdmin)
                .AsNoTracking()
                .ToListAsync();

            semaphoreSlim.Release();

            return testimonials;
        }

        public override async Task<Logger?> GetUserTestimonialsAsync()
        {
            await semaphoreSlim.WaitAsync(-1);

            var userWithTestimonials = await dbContext
               .Logger
               .Where(logger => logger.Id.ToString() == httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.SerialNumber))
               .Include(logger => logger.Testimonials)
               .FirstOrDefaultAsync();

            semaphoreSlim.Release();

            return userWithTestimonials;
        }

        public override async Task<Testimonial?> GetByIdAsync(int id)
        {
            await semaphoreSlim.WaitAsync(-1);

            var testimonial = await dbContext.Testimonial.FindAsync(id);

            semaphoreSlim.Release();

            return testimonial;
        }

        public override async Task<bool> InsertAsync(Testimonial entity)
        {
            try
            {
                await semaphoreSlim.WaitAsync(-1);

                entity.LoggerId = int.Parse(httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.SerialNumber)!);

                await dbContext.Testimonial.AddAsync(entity);
                await dbContext.SaveChangesAsync();

                semaphoreSlim.Release();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public override async Task<bool> ToggleSeenOrUnseenAsync(Testimonial entity)
        {
            try
            {
                if (entity.SeenByAdmin == true)
                    entity.SeenByAdmin = false;

                else if (entity.SeenByAdmin == false)
                    entity.SeenByAdmin = true;

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

        public override async Task<bool> DeleteAsync(Testimonial entity)
        {
            try
            {
                await semaphoreSlim.WaitAsync(-1);

                dbContext.Testimonial.Remove(entity);
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
        ~TestimonialRepository() => semaphoreSlim.Dispose();
    }
}
