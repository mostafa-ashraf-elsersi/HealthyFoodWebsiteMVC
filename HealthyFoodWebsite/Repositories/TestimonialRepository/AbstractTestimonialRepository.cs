using HealthyFoodWebsite.Models;
using System.Linq.Expressions;

namespace HealthyFoodWebsite.Repositories.TestimonialRepository
{
    public abstract class AbstractTestimonialRepository : IRepository<Testimonial>
    {
        public abstract Task<List<Testimonial>> GetAllAsync();

        public abstract Task<Testimonial?> GetByIdAsync(int id);

        public abstract Task<bool> InsertAsync(Testimonial entity);

        public Task<bool> UpdateAsync(Testimonial entity) => throw new NotImplementedException();

        public abstract Task<bool> DeleteAsync(Testimonial entity);


        // Child Object Methods Zone
        public abstract Task<Logger?> GetUserTestimonialsAsync();

        public abstract Task<bool> ToggleSeenOrUnseenAsync(Testimonial entity);
    }
}
