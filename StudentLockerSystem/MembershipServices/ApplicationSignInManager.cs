using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using StudentLockerSystem.Entities;
using StudentLockerSystem.Models;

namespace StudentLockerSystem.MembershipServices
{
    public class ApplicationSignInManager
        : SignInManager<Student>
    {
        public ApplicationSignInManager(UserManager<Student> userManager, 
            IHttpContextAccessor contextAccessor, 
            IUserClaimsPrincipalFactory<Student> claimsFactory, 
            IOptions<IdentityOptions> optionsAccessor, 
            ILogger<SignInManager<Student>> logger, 
            IAuthenticationSchemeProvider schemes,
            IUserConfirmation<Student> userConfirmation)
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, userConfirmation)
        {
        }
    }
}
