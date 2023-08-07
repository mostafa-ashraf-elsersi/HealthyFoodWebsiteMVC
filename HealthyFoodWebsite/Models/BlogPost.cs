using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthyFoodWebsite.Models
{
    public class BlogPost
    {
        [Key]
        public int Id { get; set; } = 0;

        public string PosterUri { get; set; } = string.Empty;

        [NotMapped]
        public IFormFile PosterFile { get; set; } = null!;

        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public string AuthorType { get; set; } = string.Empty;

        public string PublishingDate { get; set; } = DateTime.Now.ToString("dd/MM/yyyy");

        public bool IsDisplayed { get; set; } = true;
    }
}
