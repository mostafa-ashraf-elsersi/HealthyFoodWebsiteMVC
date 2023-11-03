using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthyFoodWebsite.Models
{
    public class Testimonial
    {
        [Key]
        public int Id { get; set; } = 0;

        public string? Content { get; set; } = string.Empty; // Content is null due to a validation trade-off.

        public byte? RatingValue { get; set; } // RatingValue is null due to a validation trade-off.

        public bool SeenByAdmin { get; set; } = false;

        [ForeignKey("Logger")]
        public int LoggerId { get; set; }


        // Navigation Properties
        public Logger? Logger { get; set; } = null!;
    }
}
