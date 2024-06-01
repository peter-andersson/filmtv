var builder = DistributedApplication.CreateBuilder(args);

var postgresPassword = builder.AddParameter("postgresql-password", secret: true);
var postgres = builder.AddPostgres("postgresServer", password: postgresPassword)
    .WithDataVolume()
    .WithImageTag("15-bookworm")
    .WithPgAdmin(c => c.WithImageTag("8.7"))
    .AddDatabase("postgres");

builder.AddProject<Projects.FilmTV_Web>("web", "https")
    .WithReference(postgres);

builder.Build().Run();