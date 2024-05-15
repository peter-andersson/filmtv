using System.Net;
using System.Net.Http.Json;
using FilmTV.Api.Common;
using FilmTV.Api.Features.Movies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FilmTV.Api.IntegrationTests.Movies;

[Collection("TestCollection")]
public class FetchMovieTest : IAsyncLifetime
{
    private readonly TestWebApplicationFactory<Program> _factory;

    public FetchMovieTest(TestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetMovie_ReturnsMovieResponse_WhenMovieExists()
    {
        // Arrange
        var client = _factory.HttpClient;
        var movieId = 1; // replace with an existing movie id

        // Act
        var response = await client.GetAsync($"/movie/{movieId}");

        // Assert
        response.EnsureSuccessStatusCode();
        var movieResponse = await response.Content.ReadFromJsonAsync<MovieResponse>();
        Assert.NotNull(movieResponse);
        Assert.Equal(movieId, movieResponse.Id);
    }

    [Fact]
    public async Task GetMovie_ReturnsNotFound_WhenMovieDoesNotExist()
    {
        // Arrange
        var client = _factory.HttpClient;
        var movieId = 999999; // replace with a non-existing movie id

        // Act
        var response = await client.GetAsync($"/movie/{movieId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    public async Task InitializeAsync()
    {
        var movie = new Movie()
        {
            MovieId = 1,
            OriginalTitle = "Test",
            OriginalLanguage = "en",
            ReleaseDate = DateTime.UtcNow,
            RunTime = 120,
            ImdbId = "1234",
            ETag = "test"
        };

        var userMovie = new UserMovie()
        {
            MovieId = 1,
            UserId = "Test",
        };

        var db = _factory.Services.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>();
        db.Movies.Add(movie);
        db.UserMovies.Add(userMovie);
        await db.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        var db = _factory.Services.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Movies.Where(m => m.MovieId == 1).ExecuteDeleteAsync();
    }
}