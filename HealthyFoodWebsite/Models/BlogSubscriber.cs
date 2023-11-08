using System.ComponentModel.DataAnnotations;

namespace HealthyFoodWebsite.Models
{
    public class BlogSubscriber
    {
        [Key]
        public int Id { get; set; } = 0;

        public string UserName { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Invalid e-mail address.")]
        [RegularExpression(@"^(?!.*\.{2})[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid e-mail address.")]
        public string EmailAddress { get; set; } = string.Empty;
    }
}
