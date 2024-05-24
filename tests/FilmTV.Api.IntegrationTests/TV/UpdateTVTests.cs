using System.Net;
using System.Net.Http.Json;
using Bogus;
using FilmTV.Api.Features.TV;

namespace FilmTV.Api.IntegrationTests.TV;

[Collection("TestCollection")]
public class UpdateTVTests : BaseTest, IAsyncLifetime
{
    private readonly TestWebApplicationFactory<Program> _factory;

    public UpdateTVTests(TestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Update_Not_Existing_Series_Should_Return_NotFound()
    {
        // Arrange
        var client = _factory.HttpClient;
        const int seriesId = 2;
        var request = new Faker<SeriesUpdateRequest>()
            .CustomInstantiator(_ => new SeriesUpdateRequest(
                _.Name.FullName(),
                _.Random.Int(1, 10)))
            .Generate();
        
        // Act
        var response = await client.PutAsJsonAsync($"/tv/{seriesId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task Update_Existing_Series_Should_Return_NoContent()
    {
        // Arrange
        var client = _factory.HttpClient;
        const int seriesId = 4;
        var request = new Faker<SeriesUpdateRequest>()
            .CustomInstantiator(_ => new SeriesUpdateRequest(
                _.Name.FullName(),
                _.Random.Int(1, 10)))
            .Generate();

        // Act
        var response = await client.PutAsJsonAsync($"/tv/{seriesId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
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