using FilmTV.Api.Features.Movies;
using Microsoft.Extensions.DependencyInjection;

namespace FilmTV.Api.IntegrationTests;

public class BaseTest
{
    public async Task SeedData(TestWebApplicationFactory<Program> factory)
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
         
        await dbContext.SaveChangesAsync();
    }
}