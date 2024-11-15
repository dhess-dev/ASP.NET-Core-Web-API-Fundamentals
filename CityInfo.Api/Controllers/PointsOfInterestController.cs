using CityInfo.Api.Models;
using CityInfo.Api.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32.SafeHandles;

namespace CityInfo.Api.Controllers;

[Route("api/cities/{cityId}/[controller]")]
[ApiController]
public class PointsOfInterestController : ControllerBase
{
    private readonly ILogger<PointsOfInterestController> _logger;
    private readonly IMailService _mailService;
    private readonly CitiesDataStore _citiesDataStore;

    public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService, CitiesDataStore citiesDataStore)
    {
        _logger = logger;
        _mailService = mailService;
        _citiesDataStore = citiesDataStore;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest(int cityId)
    {
        try
        {
            
            var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city is null)
            {
                _logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");
                return NotFound();
            }

            return Ok(city.PointsOfInterest);
        }
        catch (Exception ex)
        {
            _logger.LogCritical($"Exception while getting points of interest for city with id {cityId}.", ex);
            return StatusCode(500, "A problem happened while handling your request.");
        }
    }

    [HttpGet("{pointOfInterestId}", Name = "GetPointOfInterest")]
    public ActionResult<PointOfInterestDto> GetPointOfInterest(int cityId, int pointOfInterestId)
    {
        var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
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

    [HttpPost]
    public ActionResult<PointOfInterestDto> CreatePointsOfInterest(int cityId,
        PointOfInterestForCreationDto pointOfInterest)
    {
        var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        if (city is null)
        {
            return NotFound();
        }

        var maxPointOfInterestId = _citiesDataStore.Cities.SelectMany(c => c.PointsOfInterest).Max(p => p.Id);
        var finalPointOfInterest = new PointOfInterestDto()
        {
            Id = ++maxPointOfInterestId,
            Name = pointOfInterest.Name,
            Description = pointOfInterest.Description,
        };
        city.PointsOfInterest.Add(finalPointOfInterest);
        return CreatedAtRoute("GetPointOfInterest", new
            {
                cityId,
                pointOfInterestId = finalPointOfInterest.Id
            },
            finalPointOfInterest);
    }

    [HttpPut("{pointOfInterestId}")]
    public ActionResult UpdatePointOfInterest(int cityId, int pointOfInterestId,
        PointOfInterestForUpdateDto pointOfInterest)
    {
        var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        if (city is null)
        {
            return NotFound();
        }

        var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);
        if (pointOfInterestFromStore is null)
        {
            return NotFound();
        }

        pointOfInterestFromStore.Name = pointOfInterest.Name;
        pointOfInterestFromStore.Description = pointOfInterest.Description;

        return NoContent();
    }

    [HttpPatch("{pointOfInterestId}")]
    public ActionResult PartiallyUpdatePointOfInterest(int cityId, int pointOfInterestId,
        JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
    {
        var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        if (city is null)
        {
            return NotFound();
        }

        var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);
        if (pointOfInterestFromStore is null)
        {
            return NotFound();
        }

        var pointOfInterestToPatch = new PointOfInterestForUpdateDto()
        {
            Name = pointOfInterestFromStore.Name,
            Description = pointOfInterestFromStore.Description,
        };
        patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!TryValidateModel(pointOfInterestToPatch))
        {
            return BadRequest(ModelState);
        }

        pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
        pointOfInterestFromStore.Description = pointOfInterestToPatch.Description;
        return NoContent();
    }

    [HttpDelete("{pointOfInterestId}")]
    public ActionResult DeletePointOfInterest(int cityId, int pointOfInterestId)
    {
        var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        if (city is null)
        {
            return NotFound();
        }

        var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);
        if (pointOfInterestFromStore is null)
        {
            return NotFound();
        }

        city.PointsOfInterest.Remove(pointOfInterestFromStore);
        _mailService.Send("Point of interest deleted.",
            $"Point of interest {pointOfInterestFromStore.Name} with id {pointOfInterestFromStore.Id} was deleted.");
        return NoContent();
    }
}