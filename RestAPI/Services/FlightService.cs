using RestAPI.Services.Interfaces;
using System.Xml.Linq;

namespace RestAPI.Services
{
    public class FlightService : IFlightService
    {
        public Task<XElement> SearchFlights(string searchTerm)
        {
            throw new NotImplementedException();
        }
    }
}
