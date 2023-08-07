using System.ComponentModel.DataAnnotations;

namespace HealthyFoodWebsite.Models
{
    public class BlogSubscriber
    {
        [Key]
        public int Id { get; set; } = 0;

        public string UserName { get; set; } = "Mostafa Ashraf";

        public string EmailAddress { get; set; } = string.Empty;
    }
}
