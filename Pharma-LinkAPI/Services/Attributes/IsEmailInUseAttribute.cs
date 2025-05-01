using Microsoft.AspNetCore.Identity;
using Pharma_LinkAPI.Identity;
using System.ComponentModel.DataAnnotations;

namespace Pharma_LinkAPI.Services.Attributes
{
    public class IsEmailInUseAttribute: ValidationAttribute
    {

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if(value != null)
            {
                string email = (string)value;
                var userManager = (UserManager<AppUser>)validationContext.GetService(typeof(UserManager<AppUser>));
                var user = userManager.FindByEmailAsync(email).Result;
                if (user == null)
                {
                    return ValidationResult.Success;
                }
            }
            return new ValidationResult("Email already in use");
        }
    }
}
