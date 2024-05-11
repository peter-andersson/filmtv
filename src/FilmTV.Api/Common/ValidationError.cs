namespace FilmTV.Api.Common;

public class ValidationError(IDictionary<string, string[]> errors)
{
    public IDictionary<string, string[]> Errors { get; } = errors;
}