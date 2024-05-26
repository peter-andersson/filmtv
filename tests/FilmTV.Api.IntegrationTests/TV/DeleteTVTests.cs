// using System.Net;
//
// namespace FilmTV.Api.IntegrationTests.TV;
//
// [Collection("TestCollection")]
// public class DeleteTVTests : BaseTest, IAsyncLifetime
// {
//     private readonly TestWebApplicationFactory<Program> _factory;
//
//     public DeleteTVTests(TestWebApplicationFactory<Program> factory)
//     {
//         _factory = factory;
//     }
//
//     [Fact]
//     public async Task Delete_Not_Existing_Series_Should_Return_NoContent()
//     {
//         // Arrange
//         var client = _factory.HttpClient;
//         const int seriesId = 2;
//
//         // Act
//         var response = await client.DeleteAsync($"/tv/{seriesId}");
//
//         // Assert
//         response.StatusCode.Should().Be(HttpStatusCode.NoContent);
//     }
//     
//     [Fact]
//     public async Task Delete_Existing_Series_Should_Return_NoContent_And_Delete_The_Series_From_User_List()
//     {
//         // Arrange
//         var client = _factory.HttpClient;
//         const int seriesId = 4;
//
//         // Act
//         var response = await client.DeleteAsync($"/tv/{seriesId}");
//
//         // Assert
//         response.StatusCode.Should().Be(HttpStatusCode.NoContent);
//         
//         var getResponse = await client.GetAsync($"/tv/{seriesId}");
//         getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
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