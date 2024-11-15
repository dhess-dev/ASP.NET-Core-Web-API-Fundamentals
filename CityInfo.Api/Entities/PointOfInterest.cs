using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CityInfo.Api.Entities;

public class PointOfInterest
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public  required string Name { get; set; }
    [MaxLength(200)]
    public string? Description { get; set; }
    public City? City { get; set; }
    public int CityId { get; set; }

   
}