using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HealthyFoodWebsite.Models
{
    public class CustomerMessage
    {
        [Key]
        public int Id { get; set; } = 0;

        [RegularExpression(@"^[A-Z][a-z]{2,}\s[A-Z][a-z]{2,}$",
            ErrorMessage = "Invalid string format! Write your name of two words, each word has the first letter capitalized, and a minimum length of 3 characters; (e.g., Mostafa Ashraf).")]
        [DisplayName("(Name)")]
        public string UserName { get; set; } = string.Empty;

        [RegularExpression(@"^0(10|11|12|15)\d{8}$", ErrorMessage = "Invalid phone number! Try enter a valid one.")]
        [DisplayName(displayName: "(Phone Number)")]
        public string PhoneNumber { get; set; } = string.Empty;

        [StringLength(50, MinimumLength = 3)]
        [DisplayName("(Subject)")]
        public string Subject { get; set; } = string.Empty;

        [StringLength(300, MinimumLength = 20)]
        [DisplayName("(Message)")]
        public string Message { get; set; } = string.Empty;
    }
}
