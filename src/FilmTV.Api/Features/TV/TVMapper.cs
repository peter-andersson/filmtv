namespace FilmTV.Api.Features.TV;

public static class TVMapper
{
    public static SeriesResponse ToDto(this UserSeries userSeries)
    {
        var response = new SeriesResponse
        {
            Id = userSeries.SeriesId,
            Title = userSeries.Title ?? userSeries.Series.OriginalTitle,
            OriginalTitle = userSeries.Series.OriginalTitle,
            Status = userSeries.Series.Status,
            Rating = userSeries.Rating
        };

        foreach (var userEpisode in userSeries.Episodes)
        {
            response.Episodes.Add(new EpisodeResponse(
                userEpisode.Episode.SeasonNumber,
                userEpisode.Episode.EpisodeNumber,
                userEpisode.Episode.Title,
                userEpisode.Episode.AirDate,
                userEpisode.Watched));
        }

        return response;
    }
}