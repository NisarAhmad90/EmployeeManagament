using Microsoft.AspNetCore.Identity;

namespace EmployeeManagament.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string City { get; set; }
    }
}
