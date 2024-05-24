using System.Net;
using System.Net.Http.Json;
using FilmTV.Api.Features.TV;

namespace FilmTV.Api.IntegrationTests.TV;

[Collection("TestCollection")]
public class AddTVTests : BaseTest, IAsyncLifetime
{
    private readonly TestWebApplicationFactory<Program> _factory;

    public AddTVTests(TestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task AddTvShow_Should_Return_NotFound_For_Invalid_TvShowId()
    {
        // Arrange
        var client = _factory.HttpClient;
        const int tvShowId = 999_999_999;

        // Act
        var response = await client.PostAsync($"/tv/{tvShowId}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task AddTvShow_Should_Return_SeriesResponse_For_Valid_TvShowId()
    {
        // Arrange
        var client = _factory.HttpClient;
        const int tvShowId = 1;

        // Act
        var response = await client.PostAsync($"/tv/{tvShowId}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var seriesResponse = await response.Content.ReadFromJsonAsync<SeriesResponse>();
        seriesResponse.Should().NotBeNull();
        seriesResponse?.Id.Should().Be(tvShowId);
    }

    [Fact]
    public async Task AddTvShow_Should_Return_Conflict_For_TvShow_The_User_Already_Have_Added()
    {
        // Arrange
        var client = _factory.HttpClient;
        const int tvShowId = 4;

        // Act
        var response = await client.PostAsync($"/tv/{tvShowId}", null);

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