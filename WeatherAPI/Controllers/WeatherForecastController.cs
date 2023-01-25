using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeatherAPI.WeatherInfrastructure.WeatherRepository;

namespace WeatherAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private IWeatherRepository _weatherService;

        public WeatherForecastController(IWeatherRepository weatherService)
        {
            _weatherService = weatherService;
        }
        [HttpGet(Name = "GetWeatherForecast"), Authorize(Roles = "Admin")]
        public async Task<ActionResult> Get (string location)
        {
            return Ok(await _weatherService.GetCurrentWeatherAsync(location));
        }
    }
}
