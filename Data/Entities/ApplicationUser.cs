using Microsoft.AspNetCore.Identity;

namespace POS.Data.Entities;

public class ApplicationUser : IdentityUser
{
    public string? DisplayName { get; set; }
}
