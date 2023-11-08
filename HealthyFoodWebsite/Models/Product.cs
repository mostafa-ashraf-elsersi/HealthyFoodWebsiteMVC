using HealthyFoodWebsite.ValidationClasses;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HealthyFoodWebsite.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; } = 0;

        public string ImageUri { get; set; } = string.Empty;

        [NotMapped]
        [JsonIgnore]
        [FileSizeAndExtensions(1024 * 1024 * 5, new string[] { ".png", ".jpg" })]
        [DisplayName("(Product Image)")]
        public IFormFile? ImageFile { get; set; } = null!;

        [RegularExpression(@"^[A-Z][a-z]{2,}(\s[A-Z][a-z]{2,}){0,3}$",
           ErrorMessage = "Invalid string format! Write product name of four words at max, each word has the first letter capitalized, and a minimum length of 3 characters.")]
        [DisplayName("(Name)")]
        public string Name { get; set; } = string.Empty;

        [Range(0.0, double.PositiveInfinity, ErrorMessage = "The field (Price) must be positive.")]
        [DisplayName("(Price)")]
        public float Price { get; set; }

        [RegularExpression(@"^(?!ChooseCategory$).*$", ErrorMessage = "(Category) is not selected.")]
        [DisplayName("(Category)")]
        public string Category { get; set; } = string.Empty;

        public DateTimeOffset UploadingDateAndTime { get; set; } = DateTimeOffset.Now.LocalDateTime;

        public bool IsDisplayed { get; set; } = true;
    }
}
