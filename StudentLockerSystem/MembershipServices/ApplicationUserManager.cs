using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using StudentLockerSystem.Models;

namespace StudentLockerSystem.MembershipServices
{
    public class ApplicationUserManager
        : UserManager<Student>
    {
        public ApplicationUserManager(IUserStore<Student> store, 
            IOptions<IdentityOptions> optionsAccessor, 
            IPasswordHasher<Student> passwordHasher, 
            IEnumerable<IUserValidator<Student>> userValidators, 
            IEnumerable<IPasswordValidator<Student>> passwordValidators, 
            ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, 
            IServiceProvider services, ILogger<UserManager<Student>> logger)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }
    }
}
