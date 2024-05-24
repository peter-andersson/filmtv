# FilmTV
A minimalistic movie and tv show tracker

## Development

### Aspire dashboard
Start aspire dashboard for local OpenTelemtry logging during development.
```
docker run --rm -it -p 18888:18888 -p 4317:18889 -d --name aspire-dashboard -e DOTNET_DASHBOARD_UNSECURED_ALLOW_ANONYMOUS='true' mcr.microsoft.com/dotnet/aspire-dashboard:latest
```