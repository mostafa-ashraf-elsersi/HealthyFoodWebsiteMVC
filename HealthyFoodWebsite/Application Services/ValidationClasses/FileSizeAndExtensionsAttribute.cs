using HealthyFoodWebsite.Models;
using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;

namespace HealthyFoodWebsite.ValidationClasses
{
    public class FileSizeAndExtensionsAttribute : ValidationAttribute
    {
        // Object Fields Zone
        private readonly int maxFileSizeInBytes;
        private readonly string[] validExtensions;

        // Class Parameterized Constructor
        public FileSizeAndExtensionsAttribute(int maxFileSizeInBytes, params string[] validExtensions)
        {
            this.maxFileSizeInBytes = maxFileSizeInBytes;
            this.validExtensions = validExtensions;
        }

        // Overriding Inherited Methods
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            IFormFile? fileObjectToBeValidated = value as IFormFile;

            if (fileObjectToBeValidated != null)
            {
                var fileExtension = Path.GetExtension(fileObjectToBeValidated.FileName).ToLower();

                if (validExtensions.Contains(fileExtension) || validExtensions.Length == 0)
                {
                    if (fileObjectToBeValidated.Length <= maxFileSizeInBytes)
                    {
                        return ValidationResult.Success;
                    }
                    else
                    {
                        return new ValidationResult($"Not allowed file size! Try upload an image within {maxFileSizeInBytes / 1024} MBs.");
                    }
                }
                
                return new ValidationResult("Invalid file extension! Examples of valid extensions are (.png, .jpg).");
            }

            return ValidationResult.Success;
        }
    }
}
