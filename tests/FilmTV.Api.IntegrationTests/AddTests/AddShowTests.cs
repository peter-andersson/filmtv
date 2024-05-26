using FilmTV.Web.Common;
using FilmTV.Web.Features.Add;
using Microsoft.Extensions.DependencyInjection;
using OneOf.Types;

namespace FilmTV.Api.IntegrationTests.AddTests;

[Collection("TestCollection")]
public class AddShowTests : BaseTest, IAsyncLifetime
{
    private readonly TestWebApplicationFactory<Program> _factory;

     public AddShowTests(TestWebApplicationFactory<Program> factory)
     {
         _factory = factory;
     }

     [Fact]
     public async Task AddTvShow_Should_Return_NotFound_For_Invalid_TvShowId()
     {
         // Arrange
         const int showId = 999_999_999;
         using var scope = _factory.Services.CreateScope();
         var sut = scope.ServiceProvider.GetRequiredService<IAddHandler>();         

         // Act
         var result = await sut.AddShow(showId, UserId);

         // Assert
         result.Switch(
             response => true.Should().BeFalse(),
             notFound => notFound.Should().BeOfType<NotFound>(),
             conflict => true.Should().BeFalse()
         );
     }
     
     [Fact]
     public async Task AddTvShow_Should_Return_SeriesResponse_For_Valid_TvShowId()
     {
         // Arrange
         const int showId = 1;
         using var scope = _factory.Services.CreateScope();
         var sut = scope.ServiceProvider.GetRequiredService<IAddHandler>();         

         // Act
         var result = await sut.AddShow(showId, UserId);

         // Assert
         result.Switch(
             response => response.Title.Should().NotBeNullOrWhiteSpace(),
             notFound => true.Should().BeFalse(),
             conflict => true.Should().BeFalse()
         );
     }

     [Fact]
     public async Task AddTvShow_Should_Return_Conflict_For_TvShow_The_User_Already_Have_Added()
     {
         // Arrange
         const int showId = 4;
         using var scope = _factory.Services.CreateScope();
         var sut = scope.ServiceProvider.GetRequiredService<IAddHandler>();         

         // Act
         var result = await sut.AddShow(showId, UserId);

         // Assert
         result.Switch(
             response => true.Should().BeFalse(),
             notFound => true.Should().BeFalse(),
             conflict => conflict.Should().BeOfType<Conflict>()
         );
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