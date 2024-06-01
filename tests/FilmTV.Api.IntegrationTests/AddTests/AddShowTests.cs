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
     public async Task AddTvShow_WhenIdIs0_ShouldReturnNotFound()
     {
         const int showId = 0;
         using var scope = _factory.Services.CreateScope();
         var sut = scope.ServiceProvider.GetRequiredService<IAddShow>();         

         var result = await sut.AddShow(showId, UserId);
         
         result.Switch(
             _ => true.Should().BeFalse(),
             notFound => notFound.Should().BeOfType<NotFound>(),
             _ => true.Should().BeFalse()
         );
     }
     
     [Fact]
     public async Task AddTvShow_WhenIdIs1_ShouldReturnResponse()
     {
         const int showId = 1;
         using var scope = _factory.Services.CreateScope();
         var sut = scope.ServiceProvider.GetRequiredService<IAddShow>();         

         var result = await sut.AddShow(showId, UserId);

         result.Switch(
             response => response.Title.Should().NotBeNullOrWhiteSpace(),
             _ => true.Should().BeFalse(),
             _ => true.Should().BeFalse()
         );
     }

     [Fact]
     public async Task AddTvShow_WhenIdIs4_ShouldReturnConflict()
     {
         const int showId = 4;
         using var scope = _factory.Services.CreateScope();
         var sut = scope.ServiceProvider.GetRequiredService<IAddShow>();         

         var result = await sut.AddShow(showId, UserId);

         result.Switch(
             _ => true.Should().BeFalse(),
             _ => true.Should().BeFalse(),
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