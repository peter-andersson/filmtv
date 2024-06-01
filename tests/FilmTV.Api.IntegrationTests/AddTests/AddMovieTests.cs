using FilmTV.Web.Common;
using FilmTV.Web.Features.Add;
using Microsoft.Extensions.DependencyInjection;
using OneOf.Types;

namespace FilmTV.Api.IntegrationTests.AddTests;

[Collection("TestCollection")]
public class AddMovieTests : BaseTest, IAsyncLifetime
{
    private readonly TestWebApplicationFactory<Program> _factory;

    public AddMovieTests(TestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }
    
    [Fact]
    public async Task AddMovie_WhenIdIs1_ShouldReturnNotFound()
    {
        const int movieId = 1;
        using var scope = _factory.Services.CreateScope();
        var sut = scope.ServiceProvider.GetRequiredService<IAddMovie>();

        var result = await sut.AddMovie(movieId, UserId);
        
        result.Switch(
            _ => true.Should().BeFalse(),
            notFound => notFound.Should().BeOfType<NotFound>(),
            _ => true.Should().BeFalse()
            );
    }
    
    [Fact]
    public async Task AddMovie_WhenIdIs2_ShouldReturnResponse()
    {
        const int movieId = 2;
        using var scope = _factory.Services.CreateScope();
        var sut = scope.ServiceProvider.GetRequiredService<IAddMovie>();

        var result = await sut.AddMovie(movieId, UserId);
        
        result.Switch(
            response => response.Title.Should().NotBeNullOrWhiteSpace(),
            _ => true.Should().BeFalse(),
            _ => true.Should().BeFalse()
        );
    }
    
    [Fact]
    public async Task AddMovie_WhenIdIs3_ShouldReturnConflict()
    {
        const int movieId = 3;
        using var scope = _factory.Services.CreateScope();
        var sut = scope.ServiceProvider.GetRequiredService<IAddMovie>();        
    
        var result = await sut.AddMovie(movieId, UserId);
    
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