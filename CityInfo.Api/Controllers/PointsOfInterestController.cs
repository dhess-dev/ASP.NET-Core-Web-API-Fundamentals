using CityInfo.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.Api.Controllers;

[Route("api/cities/{cityId}/[controller]")]
[ApiController]
public class PointsOfInterestController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest(int cityId)
    {
        var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
        if (city is null)
        {
            return NotFound();
        }
        return Ok(city.PointsOfInterest);
    }

    [HttpGet("{pointOfInterestId}")]
    public ActionResult<PointOfInterestDto> GetPointOfInterest(int cityId, int pointOfInterestId)
    {
        var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
        if (city is null)
        {
            return NotFound();
        }
        
        var pointOfInterest = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);
        if (pointOfInterest is null)
        {
            return NotFound();
        }
        return Ok(pointOfInterest);
    }
}