using HealthyFoodWebsite.ValidationClasses;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
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
        [FileSizeAndExtensions(1024 * 5, new string[] { ".png", ".jpg" })]
        [DisplayName("(Post Poster)")]
        public IFormFile? PosterFile { get; set; }

        [StringLength(50, MinimumLength = 3)]
        [DisplayName("(Title)")]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000, MinimumLength = 20)]
        [DisplayName("(Content)")]
        public string Content { get; set; } = string.Empty;

        [RegularExpression(@"^(?!ChooseAuthorType$).*$", ErrorMessage = "(Author Type) is not selected.")]
        [DisplayName("(Author Type)")]
        public string AuthorType { get; set; } = string.Empty;

        public string PublishingDate { get; set; } = DateTime.Now.ToString("dd/MM/yyyy");

        public bool IsDisplayed { get; set; } = true;
    }
}
