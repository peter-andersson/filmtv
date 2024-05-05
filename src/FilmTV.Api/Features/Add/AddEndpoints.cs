namespace FilmTV.Api.Features.Add;

public static class AddEndpoints
{
    public static WebApplication MapAddEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("add")
            .RequireAuthorization("user")
            .WithTags("Add")
            .WithOpenApi();

        group.MapGet("/movie/{tmdbId}", (int id) => Results.Ok())
            .Produces(StatusCodes.Status401Unauthorized)
            .WithDescription("Add new movie")
            .WithOpenApi(generatedOperation =>
            {
                var parameter = generatedOperation.Parameters[0];
                parameter.Description = "Id for the movie from themoviedb.org";
                return generatedOperation;
            });            
            
        group.MapGet("/tv/{tmdbId}", (int id) => Results.Ok())
            .Produces(StatusCodes.Status401Unauthorized)
            .WithDescription("Add new tv show")
            .WithOpenApi(generatedOperation =>
            {
                var parameter = generatedOperation.Parameters[0];
                parameter.Description = "Id for the tv show from themoviedb.org";
                return generatedOperation;
            });

        return app;
    }    
}