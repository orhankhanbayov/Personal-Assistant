using Microsoft.AspNetCore.Identity;

namespace IdentityService.Models;

public class IdentityUserExtended : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? CreatedAt { get; set; } = DateTime.Now;

}


