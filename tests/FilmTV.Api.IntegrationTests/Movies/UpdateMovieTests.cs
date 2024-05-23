using System.Net;
using System.Net.Http.Json;
using Bogus;
using FilmTV.Api.Features.Movies;

namespace FilmTV.Api.IntegrationTests.Movies;

[Collection("TestCollection")]
public class UpdateMovieTests : BaseTest, IAsyncLifetime
{
    private readonly TestWebApplicationFactory<Program> _factory;

    public UpdateMovieTests(TestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Update_Not_Existing_Movie_Should_Return_NotFound()
    {
        // Arrange
        var client = _factory.HttpClient;
        const int movieId = 1;
        var request = new Faker<UpdateMovieRequest>()
            .CustomInstantiator(_ => new UpdateMovieRequest(
                movieId,
                _.Name.FullName(),
                _.Date.Recent(),
                _.Random.Int(1, 10)))
            .Generate();
        
        // Act
        var response = await client.PutAsJsonAsync($"/movie/{movieId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task Update_Existing_Movie_Should_Return_MovieResponse()
    {
        // Arrange
        var client = _factory.HttpClient;
        const int movieId = 3;
        var request = new Faker<UpdateMovieRequest>()
            .CustomInstantiator(_ => new UpdateMovieRequest(
                movieId,
                _.Name.FullName(),
                _.Date.Recent(),
                _.Random.Int(1, 10)))
            .Generate();

        // Act
        var response = await client.PutAsJsonAsync($"/movie/{movieId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var movieResponse = await response.Content.ReadFromJsonAsync<MovieResponse>();
        movieResponse.Should().NotBeNull();
        movieResponse?.Id.Should().Be(movieId);
        movieResponse?.Rating.Should().Be(request.Rating);
        movieResponse?.WatchedDate.Should().Be(request.WatchedDate);
        movieResponse?.Title.Should().Be(request.Title);
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