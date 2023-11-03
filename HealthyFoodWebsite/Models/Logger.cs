using HealthyFoodWebsite.ValidationClasses;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthyFoodWebsite.Models
{
    public class Logger
    {
        [Key]
        public int Id { get; set; } = 0;

        [RegularExpression(@"^[a-z]{3,}_[a-z]{3,}$", ErrorMessage = "Invalid string format! Example of a valid format is (mostafa_ashraf).")]
        [UniqueUsername]
        [DisplayName("(Username)")]
        public string Username { get; set; } = string.Empty;

        [RegularExpression(@"^[A-Z][a-z]{2,}\s[A-Z][a-z]{2,}$",
           ErrorMessage = "Invalid string format! Write your name of two words, each word has the first letter capitalized, and a minimum length of 3 characters; (e.g., Amr Ayman).")]
        [DisplayName("(Full Name)")]
        public string FullName { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Invalid e-mail address.")]
        [RegularExpression(@"^(?!.*\.{2})[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid e-mail address.")]
        [DisplayName("(Email)")]
        public string Email { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$",
            ErrorMessage = "Password must be of length 8 characters and contain at least, one lowercase character, one uppercase character, and one digit.")]
        [Compare("ConfirmingPassword")]
        [DisplayName("(Password)")]
        public string Password { get; set; } = string.Empty;

        [NotMapped]
        [DataType(DataType.Password)]
        [Compare("Password")]
        [DisplayName("(Confirming Password)")]
        public string ConfirmingPassword { get; set; } = string.Empty;

        [RegularExpression(@"^0(10|11|12|15)\d{8}$", ErrorMessage = "Invalid phone number! Try enter a valid one.")]
        [DisplayName("(Phone Number)")]
        public string PhoneNumber { get; set; } = string.Empty;

        [RegularExpression(@"^(?!ChooseARole$).*$", ErrorMessage = "(Role) is not selected.")]
        [DisplayName("(Role)")]
        public string Role { get; set; } = "User";

        public bool IsActive { get; set; } = true;

        public bool IsDeleted { get; set; } = false;


        // Navigation Properties
        public ICollection<ShoppingBagItem>? ShoppingBags { get; set; } = new List<ShoppingBagItem>();

        public ICollection<Testimonial>? Testimonials { get; set; } = new List<Testimonial>();

        public ICollection<Order>? Orders { get; set; } = new List<Order>();
    }
}
