using FilmTV.Api.Features.Movies;
using Microsoft.EntityFrameworkCore;

// Disable as we want the partial class to be in the same namespace as the original class
// ReSharper disable once CheckNamespace
namespace FilmTV.Api.Common;

public partial class AppDbContext
{
    public DbSet<Movie> Movies { get; set; } = null!;

    public DbSet<UserMovie> UserMovies { get; set; } = null!;
}