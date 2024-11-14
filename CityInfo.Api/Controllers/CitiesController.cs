using CityInfo.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CitiesController : ControllerBase
{
    private readonly CitiesDataStore _citiesDataStore;
    public CitiesController(CitiesDataStore citiesDataStore) => _citiesDataStore = citiesDataStore;

    [HttpGet]
    public ActionResult<IEnumerable<CityDto>> GetCities()
    {
        return Ok(_citiesDataStore.Cities);
    }

    [HttpGet("{id}")]
    public ActionResult<CityDto> GetCity(int id)
    {
        var cityToReturn = _citiesDataStore.Cities.FirstOrDefault(x => x.Id == id);

        if (cityToReturn is null)
        {
            return NotFound();
        }

        return Ok(cityToReturn);
    }
}