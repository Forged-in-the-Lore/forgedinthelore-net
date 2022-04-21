using Microsoft.AspNetCore.Identity;

namespace forgedinthelore_net.Entities;

public class AppRole : IdentityRole<int>
{
    public ICollection<AppUserRole> UserRoles { get; set; }
}