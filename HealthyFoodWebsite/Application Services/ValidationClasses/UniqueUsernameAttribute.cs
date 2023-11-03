using HealthyFoodWebsite.Repositories.LoggerRepository;
using HealthyFoodWebsite.Models;
using System.ComponentModel.DataAnnotations;

namespace HealthyFoodWebsite.ValidationClasses
{
    public class UniqueUsernameAttribute : ValidationAttribute
    {
        // Overriding Inherited Methods
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            Logger? loggerObjectFromRequest = validationContext.ObjectInstance as Logger;

            var loggerRepository = validationContext.GetRequiredService<AbstractLoggerRepository>();
            

            // Registration and Update Cases

            if (loggerObjectFromRequest != null)
            {
                var logger = loggerRepository.GetLoggerWithSameUsernameOrNull(loggerObjectFromRequest.Username).Result;

                if (logger == null)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    if (logger.Id == loggerObjectFromRequest.Id)
                    {
                        return ValidationResult.Success;
                    }
                    else
                    {
                        return new ValidationResult("This username has taken before! Try enter another one.");
                    }
                }
                    
            }

            return new ValidationResult("An error occurred in the server.");
        }
    }
}
