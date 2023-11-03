using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HealthyFoodWebsite.ViewModels
{
    public class LoggerCredentialsParameters
    {
        [DisplayName("(Username)")]
        public string Username { get; set; } = string.Empty;

        [DisplayName("(Email Address)")]
        public string EmailAddress { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [DisplayName("(Password)")]
        public string Password { get; set; } = string.Empty;
    }
}
