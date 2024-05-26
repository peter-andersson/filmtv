using FilmTV.Web.Data;
using Microsoft.Extensions.DependencyInjection;

namespace FilmTV.Api.IntegrationTests;

public class BaseTest
{
    public string UserId => "Test1";
    
    protected async Task SeedData(TestWebApplicationFactory<Program> factory)
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Create test users
        var user1 = new ApplicationUser() { Id = UserId, UserName = "TestUser1", Email = "testuser1@example.com" };
        dbContext.Users.Add(user1);
        
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
            ApplicationUserId = "Test1",
        };
        dbContext.UserMovies.Add(userMovie);

        var show = new Show()
        {
            ShowId = 4,
            OriginalTitle = "Test Series",
            Status = "Ongoing",
            ETag = "test",
            NextUpdate = DateTime.UtcNow.AddDays(7)
        };
        dbContext.Shows.Add(show);

        var episode = new Episode()
        {
            Show = show,
            SeasonNumber = 1,
            EpisodeNumber = 1,
            AirDate = DateTime.UtcNow.AddDays(-1),
            Title = "test episode"
        };
        show.Episodes.Add(episode);

        var userShow = new UserShow()
        {
            Show = show,
            ApplicationUserId = "Test1"
        };
        dbContext.UserShows.Add(userShow);

        var userEpisode = new UserEpisode()
        {
            Episode = episode,
            UserShow = userShow,
            Watched = false
        };
        userShow.UserEpisodes.Add(userEpisode);

        await dbContext.SaveChangesAsync();
    }
}