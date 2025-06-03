using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestAPI.Models;
using RestAPI.Validation;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore;

namespace RestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlightController : Controller
    {
        private readonly FlightsDbContext _context;
        private readonly string _xsdPath;
        private readonly string _rngPath;
        public FlightController(IWebHostEnvironment env, FlightsDbContext context)
        {
            _xsdPath = Path.Combine(env.ContentRootPath, "Validation Files", "FlightXSD.xml");
            _rngPath = Path.Combine(env.ContentRootPath, "Validation Files", "FlightRNG.xml");
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpPost("validate-xsd")]
        public async Task<IActionResult> ValidateXSD(IFormFile xmlFile)
        {
            if (xmlFile == null || xmlFile.Length == 0)
            {
                return BadRequest("No XML file provided.");
            }

            using var stream = xmlFile.OpenReadStream();
            var result = XmlValidator.ValidateWithXsd(stream, _xsdPath);
            if(result != "XSD validation passed.")
            {
                return BadRequest(result);
            }

            stream.Position = 0; // Reset stream position for further processing if needed

            try
            {
                var serializer = new XmlSerializer(typeof(FlightInfo));
                var flightInfo = (FlightInfo)serializer.Deserialize(stream);
                _context.Flights.Add(flightInfo);
                await _context.SaveChangesAsync();
                return Ok("XSD validation passed and flight information saved successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost("validate-rng")]
        public async Task<IActionResult> ValidateRNG(IFormFile xmlFile)
        {
            if (xmlFile == null || xmlFile.Length == 0)
            {
                return BadRequest("No XML file provided.");
            }

            using var stream = xmlFile.OpenReadStream();
            var result = XmlValidator.ValidateWithRng(stream, _rngPath);
            if(result != "RNG validation passed.")
            {
                return BadRequest(result);
            }

            stream.Position = 0; // Reset stream position for further processing if needed

            try
            {
                var serializer = new XmlSerializer(typeof(FlightInfo));
                var flightInfo = (FlightInfo)serializer.Deserialize(stream);
                _context.Flights.Add(flightInfo);
                await _context.SaveChangesAsync();
                return Ok("RNG validation passed and flight information saved successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
