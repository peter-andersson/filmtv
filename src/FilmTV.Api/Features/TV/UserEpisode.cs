using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilmTV.Api.Features.TV;

public class UserEpisode
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int EpisodeId { get; set; }
    [ForeignKey(nameof(EpisodeId))]
    public Episode Episode { get; set; } = null!;
    
    public bool Watched { get; set; }
}