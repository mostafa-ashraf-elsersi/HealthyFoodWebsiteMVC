using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthyFoodWebsite.Models
{
    public class Logger
    {
        [Key]
        public int Id { get; set; } = 0;

        public string Username { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string? Email { get; set; } = string.Empty;

        [PasswordPropertyText]
        public string Password { get; set; } = string.Empty;

        [NotMapped]
        public string ConfirmingPassword { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Role { get; set; } = "User";

        public bool IsActive { get; set; } = true;

        public bool IsDeleted { get; set; } = false;


        // Navigation Properties
        public ICollection<ShoppingBagItem>? ShoppingBags { get; set; } = new List<ShoppingBagItem>();

        public ICollection<Testimonial>? Testimonials { get; set; } = new List<Testimonial>();

        public ICollection<Order>? Orders { get; set; } = new List<Order>();
    }
}
