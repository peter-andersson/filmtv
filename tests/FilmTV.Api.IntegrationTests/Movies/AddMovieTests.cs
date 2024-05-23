using System.Net;
using System.Net.Http.Json;
using FilmTV.Api.Features.Movies;

namespace FilmTV.Api.IntegrationTests.Movies;

[Collection("TestCollection")]
public class AddMovieTests : BaseTest, IAsyncLifetime
{
    private readonly TestWebApplicationFactory<Program> _factory;

    public AddMovieTests(TestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task AddMovie_Should_Return_NotFound_For_Invalid_MovieId()
    {
        // Arrange
        var client = _factory.HttpClient;
        const int movieId = 1;

        // Act
        var response = await client.PostAsync($"/movie/{movieId}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task AddMovie_Should_Return_MovieResponse_For_Valid_MovieId()
    {
        // Arrange
        var client = _factory.HttpClient;
        const int movieId = 2;

        // Act
        var response = await client.PostAsync($"/movie/{movieId}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var movieResponse = await response.Content.ReadFromJsonAsync<MovieResponse>();
        movieResponse.Should().NotBeNull();
        movieResponse?.Id.Should().Be(movieId);
    }

    [Fact]
    public async Task AddMovie_Should_Return_Conflict_For_Movie_The_User_Already_Have_Added()
    {
        // Arrange
        var client = _factory.HttpClient;
        const int movieId = 3;

        // Act
        var response = await client.PostAsync($"/movie/{movieId}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    public async Task InitializeAsync()
    {
        await SeedData(_factory);
    }

    public async Task DisposeAsync()
    {
        await _factory.ResetDatabase();
    }
}