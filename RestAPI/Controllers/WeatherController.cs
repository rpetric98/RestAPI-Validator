using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestAPI.Services.Interfaces;

namespace RestAPI.Controllers
{
    [Route("[controller]")]
    public class WeatherController : Controller
    {
        private readonly IWeatherService _weatherService;

        public WeatherController(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var cities = await _weatherService.GetCitiesAsync();
            return View(cities);
        }

        [HttpGet("GetTemperature")]
        public IActionResult GetTemperature(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                return Json(new { error = "City parameter is required." });

            var temperature = _weatherService.getTemperature(city);

            if (double.IsNaN(temperature))
                return Json(new { error = "City not found or temperature data is missing." });

            return Json(new { temperature });
        }


    }
}

