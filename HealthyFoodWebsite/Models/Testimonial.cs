using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthyFoodWebsite.Models
{
    public class Testimonial
    {
        [Key]
        public int Id { get; set; } = 0;

        public string Content { get; set; } = string.Empty;

        public byte RatingValue { get; set; }

        [ForeignKey("Logger")]
        public int LoggerId { get; set; }

        public bool IsDeleted { get; set; } = false;


        // Navigation Properties
        public Logger Logger { get; set; } = null!;
    }
}
