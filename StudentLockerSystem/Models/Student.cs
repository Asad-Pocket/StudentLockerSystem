using Microsoft.AspNetCore.Identity;

namespace StudentLockerSystem.Models
{
    public class Student : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? FolderPath { get; set; }
    }
}
