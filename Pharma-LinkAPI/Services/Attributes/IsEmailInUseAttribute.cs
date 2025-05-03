using Microsoft.AspNetCore.Identity;
using Pharma_LinkAPI.Identity;
using Pharma_LinkAPI.Models;
using Pharma_LinkAPI.Repositries.Irepositry;
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
                var x = (IrequestRepositry)validationContext.GetService(typeof(IrequestRepositry));

                var request = x.GetUserByEmail(email);
                var user = userManager.Users.Any(u => u.Email == email);
                if (user == true && request == null)
                {
                    return ValidationResult.Success;
                }
            }
            return new ValidationResult("Email already in use");
        }
    }
}
