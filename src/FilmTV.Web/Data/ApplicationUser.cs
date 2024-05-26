using Microsoft.AspNetCore.Identity;
// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength

namespace FilmTV.Web.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public string? DisplayName { get; set; }

    public ICollection<UserMovie> Movies { get; set; } = [];
}