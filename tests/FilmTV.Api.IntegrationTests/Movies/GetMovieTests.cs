// using System.Net;
// using System.Net.Http.Json;
// using FilmTV.Api.Features.Movies;
//
// namespace FilmTV.Api.IntegrationTests.Movies;
//
// [Collection("TestCollection")]
// public class GetMovieTests : BaseTest, IAsyncLifetime
// {
//     private readonly TestWebApplicationFactory<Program> _factory;
//
//     public GetMovieTests(TestWebApplicationFactory<Program> factory)
//     {
//         _factory = factory;
//     }
//
//     [Fact]
//     public async Task GetMovie_Should_Return_MovieResponse()
//     {
//         // Arrange
//         var client = _factory.HttpClient;
//         const int movieId = 3;
//
//         // Act
//         var response = await client.GetAsync($"/movie/{movieId}");
//
//         // Assert
//         response.StatusCode.Should().Be(HttpStatusCode.OK);
//         var movieResponse = await response.Content.ReadFromJsonAsync<MovieResponse>();
//         Assert.NotNull(movieResponse);
//         Assert.Equal(movieId, movieResponse.Id);
//     }
//
//     [Fact]
//     public async Task GetMovie_Should_Return_NotFound()
//     {
//         // Arrange
//         var client = _factory.HttpClient;
//         const int movieId = 999999; // replace with a non-existing movie id
//
//         // Act
//         var response = await client.GetAsync($"/movie/{movieId}");
//
//         // Assert
//         response.StatusCode.Should().Be(HttpStatusCode.NotFound);
//     }
//     
//     [Fact]
//     public async Task GetWatchlist_Should_Return_List_Of_Movies()
//     {
//         // Arrange
//         var client = _factory.HttpClient;
//
//         // Act
//         var response = await client.GetAsync($"/movie/watchlist");
//
//         // Assert
//         response.StatusCode.Should().Be(HttpStatusCode.OK);
//         var watchlist = await response.Content.ReadFromJsonAsync<List<WatchlistMovieResponse>>();
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