using System.Net;
using System.Net.Http.Json;
using FilmTV.Api.Features.Movies;
using Microsoft.Extensions.DependencyInjection;

namespace FilmTV.Api.IntegrationTests.Movies;

[Collection("TestCollection")]
public class DeleteMovieTests : BaseTest, IAsyncLifetime
{
    private readonly TestWebApplicationFactory<Program> _factory;

    public DeleteMovieTests(TestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Delete_Not_Existing_Movie_Should_Return_NoContent()
    {
        // Arrange
        var client = _factory.HttpClient;
        const int movieId = 1;

        // Act
        var response = await client.DeleteAsync($"/movie/{movieId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
    
    [Fact]
    public async Task Delete_Existing_Movie_Should_Return_NoContent_And_Delete_The_Movie_From_User_List()
    {
        // Arrange
        var client = _factory.HttpClient;
        const int movieId = 3;

        // Act
        var response = await client.DeleteAsync($"/movie/{movieId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        var getResponse = await client.GetAsync($"/movie/{movieId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
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