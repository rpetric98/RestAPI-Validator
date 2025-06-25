using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestAPI.Models;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore;
using Commons.Xml.Relaxng;
using System.Xml.Schema;
using System.IO;

namespace RestAPI.Controllers
{
    [Controller]
    [Route("[controller]/[action]")]
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

        [HttpPost]
        public async Task<IActionResult> ValidateXSD(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                using (var stream = file.OpenReadStream())
                {
                    xmlDoc.Load(stream);
                }

                var validationErrors = new List<string>();
                xmlDoc.Schemas.Add(null, _xsdPath);
                xmlDoc.Validate((sender, e) =>
                {
                    validationErrors.Add(e.Message);
                });

                if (validationErrors.Count > 0)
                {
                    return BadRequest(new { Errors = validationErrors });
                }

                var serializer = new XmlSerializer(typeof(FlightDetails));
                FlightDetails? flightDetails;
                using (var nodeReader = new XmlNodeReader(xmlDoc.DocumentElement))
                {
                    flightDetails = (FlightDetails?)serializer.Deserialize(nodeReader);
                }

                if (flightDetails == null)
                    return BadRequest("Could not deserialize the XML into FlightDetails object.");

                _context.FlightDetails.Add(flightDetails);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Flight details saved successfully." });
            }
            catch (XmlSchemaValidationException xsdEx)
            {
                return BadRequest(new { Errors = new[] { xsdEx.Message } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ValidateRNG(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { Error = "No file uploaded." });

            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                using (var stream = file.OpenReadStream())
                {
                    xmlDoc.Load(stream);
                }

                var validationErrors = new List<string>();

                using (var rngReader = new RelaxngValidatingReader(new XmlNodeReader(xmlDoc), new XmlTextReader(_rngPath)))
                {
                    try
                    {
                        while (rngReader.Read()) { }
                    }
                    catch (XmlSchemaValidationException valEx)
                    {
                        validationErrors.Add(valEx.Message);
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, new { Error = "RNG validation failed: " + ex.Message });
                    }
                }

                if (validationErrors.Count > 0)
                {
                    return BadRequest(new { Errors = validationErrors });
                }

                var serializer = new XmlSerializer(typeof(FlightDetails));
                FlightDetails? flightDetails;
                using (var nodeReader = new XmlNodeReader(xmlDoc.DocumentElement))
                {
                    flightDetails = (FlightDetails?)serializer.Deserialize(nodeReader);
                }

                if (flightDetails == null)
                    return BadRequest(new { Error = "Could not deserialize the XML into FlightDetails object." });

                _context.FlightDetails.Add(flightDetails);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Flight details saved successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }
    }
}
