// using System.Net;
// using System.Net.Http.Json;
// using FilmTV.Api.Features.TV;
//
// namespace FilmTV.Api.IntegrationTests.TV;
//
// [Collection("TestCollection")]
// public class GetTVTests : BaseTest, IAsyncLifetime
// {
//     private readonly TestWebApplicationFactory<Program> _factory;
//
//     public GetTVTests(TestWebApplicationFactory<Program> factory)
//     {
//         _factory = factory;
//     }
//
//     [Fact]
//     public async Task GetSeries_Should_Return_SeriesResponse()
//     {
//         // Arrange
//         var client = _factory.HttpClient;
//         const int seriesId = 4;
//
//         // Act
//         var response = await client.GetAsync($"/tv/{seriesId}");
//
//         // Assert
//         response.StatusCode.Should().Be(HttpStatusCode.OK);
//         var seriesResponse = await response.Content.ReadFromJsonAsync<SeriesResponse>();
//         Assert.NotNull(seriesResponse);
//         Assert.Equal(seriesId, seriesResponse.Id);
//     }
//
//     [Fact]
//     public async Task GetSeries_Should_Return_NotFound()
//     {
//         // Arrange
//         var client = _factory.HttpClient;
//         const int seriesId = 999999; // replace with a non-existing movie id
//
//         // Act
//         var response = await client.GetAsync($"/tv/{seriesId}");
//
//         // Assert
//         response.StatusCode.Should().Be(HttpStatusCode.NotFound);
//     }
//     
//     [Fact]
//     public async Task GetWatchlist_Should_Return_List_Of_Series()
//     {
//         // Arrange
//         var client = _factory.HttpClient;
//
//         // Act
//         var response = await client.GetAsync($"/tv/watchlist");
//
//         // Assert
//         response.StatusCode.Should().Be(HttpStatusCode.OK);
//         var watchlist = await response.Content.ReadFromJsonAsync<List<WatchlistSeriesResponse>>();
//         watchlist.Should().NotBeNull();
//         watchlist.Should().NotBeEmpty();
//     }    
//
//     public async Task InitializeAsync()
//     {
//         await SeedData(_factory);
//     }
//
//     public async Task DisposeAsync()
//     {
//         await _factory.ResetDatabase();
//     }
// }