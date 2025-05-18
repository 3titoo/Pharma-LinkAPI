using Microsoft.AspNetCore.Identity;
using Pharma_LinkAPI.Identity;
using Pharma_LinkAPI.Repositries.Irepositry;
using System.ComponentModel.DataAnnotations;

namespace Pharma_LinkAPI.Services.Attributes
{
    public class IsUserNameInUseAttribute : ValidationAttribute
    {

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string username = (string)value;
                var userManager = (UserManager<AppUser>)validationContext.GetService(typeof(UserManager<AppUser>));
                var x = (IrequestRepositry)validationContext.GetService(typeof(IrequestRepositry));
                var request = x.GetUserByusername(username).Result;
                var user = userManager.Users.Any(u => u.UserName == username);
                if (user == false && request == null)
                {
                    return ValidationResult.Success;
                }
            }
            return new ValidationResult("User Name already in use");
        }
    }
}
