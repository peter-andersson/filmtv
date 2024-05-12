using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilmTV.Api.Features.TV;

public class UserShow
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int ShowId { get; set; }
    [ForeignKey(nameof(ShowId))]
    public Show Show { get; set; } = null!;
    
    public string? Title { get; set; }
    
    public int Rating { get; set; }
    
    public DateTime? RatingDate { get; set; }
}