// ReSharper disable UnusedParameter.Local
// ReSharper disable ConvertClosureToMethodGroup

using FilmTV.Api.Common;

namespace FilmTV.Api.Features.Images;

public static class ImageApi
{
    public static void MapImageRoutes(this IEndpointRouteBuilder app)
    {
        app.MapGroup("/image")
            .MapImageApi()
            .WithTags("Image")
            .WithOpenApi();
    }
    
    private static RouteGroupBuilder MapImageApi(this RouteGroupBuilder group)
    {
        group.MapGet("movie/{strImage}", (string strImage, HttpContext http, CancellationToken token) =>
        {
            http.Response.Headers.CacheControl = $"public,max-age={TimeSpan.FromDays(30).TotalSeconds}";
            var filename = $"{ImagePath.Directory}/movie/{strImage}";
            return File.Exists(filename) ? Results.File(filename, "image/jpeg") : Results.NotFound();
        })
        .Produces(StatusCodes.Status404NotFound);
        
        group.MapGet("tv/{strImage}", (string strImage, HttpContext http, CancellationToken token) =>
        {
            http.Response.Headers.CacheControl = $"public,max-age={TimeSpan.FromDays(30).TotalSeconds}";
            var filename = $"{ImagePath.Directory}/tv/{strImage}";
            return File.Exists(filename) ? Results.File(filename, "image/jpeg") : Results.NotFound();            
        })
        .Produces(StatusCodes.Status404NotFound);

        return group;
    }
}