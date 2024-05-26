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
    public async Task AddMovie_Should_Return_NotFound_For_Invalid_MovieId()
    {
        // Arrange
        const int movieId = 1;
        using var scope = _factory.Services.CreateScope();
        var sut = scope.ServiceProvider.GetRequiredService<IAddHandler>();

        // Act
        var result = await sut.AddMovie(movieId, UserId);
        
        // Assert
        result.Switch(
            response => true.Should().BeFalse(),
            notFound => notFound.Should().BeOfType<NotFound>(),
            conflict => true.Should().BeFalse()
            );
    }
    
    [Fact]
    public async Task AddMovie_Should_Return_MovieResponse_For_Valid_MovieId()
    {
        // Arrange
        const int movieId = 2;
        using var scope = _factory.Services.CreateScope();
        var sut = scope.ServiceProvider.GetRequiredService<IAddHandler>();

        // Act
        var result = await sut.AddMovie(movieId, UserId);
        
        // Assert
        result.Switch(
            response => response.Title.Should().NotBeNullOrWhiteSpace(),
            notFound => true.Should().BeFalse(),
            conflict => true.Should().BeFalse()
        );
    }
    
    [Fact]
    public async Task AddMovie_Should_Return_Conflict_For_Movie_The_User_Already_Have_Added()
    {
        // Arrange
        const int movieId = 3;
        using var scope = _factory.Services.CreateScope();
        var sut = scope.ServiceProvider.GetRequiredService<IAddHandler>();        
    
        // Act
        var result = await sut.AddMovie(movieId, UserId);
    
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