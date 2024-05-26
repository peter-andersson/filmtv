using System.Text.RegularExpressions;

namespace FilmTV.Web.Features.Add;

public static partial class RegexHelper
{
    [GeneratedRegex(@"^https://www.themoviedb.org/(tv|movie)/(\d+).*", RegexOptions.IgnoreCase, "en-us")]
    public static partial Regex TMDbRegex();
}