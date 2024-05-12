using FilmTV.Api.Features.TV;
using Microsoft.EntityFrameworkCore;

// Disable as we want the partial class to be in the same namespace as the original class
// ReSharper disable once CheckNamespace
namespace FilmTV.Api.Common;

public partial class AppDbContext
{
    public DbSet<Show> Shows { get; set; } = null!;
    
    public DbSet<UserShow> UserShows { get; set; } = null!;

    public DbSet<Episode> Episodes { get; set; } = null!;
    
    public DbSet<UserEpisode> UserEpisodes { get; set; } = null!;
}