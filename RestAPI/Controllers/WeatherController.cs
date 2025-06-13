using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestAPI.Services.Interfaces;

namespace RestAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherService _weatherService;

        public WeatherController(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [Authorize]
        [HttpGet]
        [Route("getCities")]
        public IActionResult GetCities()
        {
            try
            {
                var cities = _weatherService.GetCities();
                return Ok(cities);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("XML-RPC")]
        public IActionResult XmlRpc([FromBody] string city)
        {
            try
            {
                var temperature = _weatherService.getTemperature(city);
                var response = $"<?xml version=\"1.0\"?><methodResponse><params><param><value><double>{temperature}</double></value></param></params></methodResponse>";
                return Content(response, "text/xml");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
