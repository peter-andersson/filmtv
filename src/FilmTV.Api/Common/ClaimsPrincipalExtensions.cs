using System.Security.Claims;

namespace FilmTV.Api.Common;

public static class ClaimsPrincipalExtensions
{
    public static string GetUserId(this ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);
        
        var claim = principal.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null ? claim.Value : string.Empty;
    }
}