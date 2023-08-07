using System.ComponentModel.DataAnnotations;

namespace HealthyFoodWebsite.Models
{
    public class CustomerMessage
    {
        [Key]
        public int Id { get; set; } = 0;

        public string UserName { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
    }
}
