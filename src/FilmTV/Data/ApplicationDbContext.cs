using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FilmTV.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Movie> Movies { get; set; } = null!;
    
    public DbSet<UserMovie> UserMovies { get; set; } = null!;
    
    public DbSet<TVShow> TVShows { get; set; } = null!;
    
    public DbSet<Episode> Episodes { get; set; } = null!;
    
    public DbSet<UserTVShow> UserTVShows { get; set; } = null!;
    
    public DbSet<UserEpisode> UserEpisodes { get; set; } = null!;
}