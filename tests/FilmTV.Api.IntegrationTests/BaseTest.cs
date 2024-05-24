using FilmTV.Api.Features.Movies;
using FilmTV.Api.Features.TV;
using Microsoft.Extensions.DependencyInjection;

namespace FilmTV.Api.IntegrationTests;

public class BaseTest
{
    protected static async Task SeedData(TestWebApplicationFactory<Program> factory)
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Create test users
        var user1 = new AppUser() { Id = "Test1", UserName = "TestUser1", Email = "testuser1@example.com" };
        dbContext.Users.Add(user1);

        var user2 = new AppUser() { Id = "Test2", UserName = "TestUser2", Email = "testuser2@example.com" };
        dbContext.Users.Add(user2);

        var movie = new Movie()
        {
            MovieId = 3,
            OriginalTitle = "Test",
            OriginalLanguage = "en",
            ReleaseDate = DateTime.UtcNow,
            RunTime = 120,
            ImdbId = "1234",
            ETag = "test"
        };
        dbContext.Movies.Add(movie);

        var userMovie = new UserMovie()
        {
            MovieId = 3,
            UserId = "Test1",
        };
        dbContext.UserMovies.Add(userMovie);

        var series = new Series()
        {
            Id = 4,
            OriginalTitle = "Test Series",
            Status = "Ongoing",
            ETag = "test",
            NextUpdate = DateTime.UtcNow.AddDays(7)
        };
        dbContext.Series.Add(series);

        var episode = new Episode()
        {
            SeriesId = 4,
            SeasonNumber = 1,
            EpisodeNumber = 1,
            AirDate = DateTime.UtcNow.AddDays(-1),
            Title = "test episode"
        };
        dbContext.Episodes.Add(episode);

        var userSeries = new UserSeries()
        {
            SeriesId = 4,
            UserId = "Test1"
        };
        dbContext.UserSeries.Add(userSeries);

        var userEpisode = new UserEpisode()
        {
            UserSeries = userSeries,
            Episode = episode,
            UserId = "Test1",
            Watched = false
        };
        dbContext.UserEpisodes.Add(userEpisode);

        await dbContext.SaveChangesAsync();
    }
}