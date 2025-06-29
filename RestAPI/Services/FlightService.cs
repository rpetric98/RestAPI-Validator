using Microsoft.EntityFrameworkCore;
using RestAPI.Models;
using RestAPI.Services.Interfaces;
using System.Xml.Linq;
using System.Xml.XPath;

namespace RestAPI.Services
{
    public class FlightService : IFlightService
    {

        private readonly FlightsDbContext _context;

        public FlightService(FlightsDbContext context)
        {
            _context = context;
        }

        public async Task<string> SearchFlights(string searchTerm)
        {
            var xml = await GenerateXml();
            var results = SearchXml(xml, searchTerm);
            return results.ToString();
        }

        private XElement SearchXml(XDocument xml, string searchTerm)
        {
            var results = xml.XPathSelectElements(
       $"//Flight[contains(Market, '{searchTerm}') or contains(Legs/Leg/Origin, '{searchTerm}') or contains(Legs/Leg/Destination, '{searchTerm}')]"
   );
            return new XElement("Flights", results);
        }

        private async Task<XDocument> GenerateXml()
        {
            var flights = await _context.FlightDetails
         .Include(f => f.Legs)
         .ToListAsync();

            var xml = new XDocument(
                new XElement("Flights",
                    from flight in flights
                    select new XElement("Flight",
                        new XElement("Adults", flight.Adults),
                        new XElement("Currency", flight.Currency),
                        new XElement("Locale", flight.Locale),
                        new XElement("Market", flight.Market),
                        new XElement("CabinClass", flight.CabinClass),
                        new XElement("CountryCode", flight.CountryCode),
                        new XElement("Legs",
                            from leg in flight.Legs
                            select new XElement("Leg",
                                new XElement("Origin", leg.Origin),
                                new XElement("Destination", leg.Destination),
                                new XElement("Date", leg.Date.ToString("o")) // ISO 8601 format
                            )
                        )
                    )
                )
            );
            xml.Save("flights.xml");

            return xml;
        }


    }
}
