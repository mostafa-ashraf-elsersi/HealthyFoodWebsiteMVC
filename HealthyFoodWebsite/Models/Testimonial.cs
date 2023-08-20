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

        public bool SeenByAdmin { get; set; } = false;

        [ForeignKey("Logger")]
        public int LoggerId { get; set; }


        // Navigation Properties
        public Logger Logger { get; set; } = null!;
    }
}
