using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CityInfo.Api.Entities;

public class City
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public required string Name { get; set; }

    [MaxLength(200)]
    public string? Description { get; set; }

    public ICollection<PointOfInterest> PointsOfInterest { get; set; } = new List<PointOfInterest>();
}