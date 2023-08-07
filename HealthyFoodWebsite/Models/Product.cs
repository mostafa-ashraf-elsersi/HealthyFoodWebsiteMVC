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
        public IFormFile ImageFile { get; set; } = null!;

        public string Name { get; set; } = string.Empty;

        public float Price { get; set; }
     
        public string Category { get; set; } = string.Empty;

        public DateTimeOffset UploadingDateAndTime { get; set; } = DateTimeOffset.Now.LocalDateTime;

        public bool IsDisplayed { get; set; } = true;
    }
}
