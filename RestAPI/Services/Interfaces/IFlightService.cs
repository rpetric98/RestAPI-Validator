using System.ServiceModel;
using System.Xml.Linq;

namespace RestAPI.Services.Interfaces
{
    [ServiceContract]
    public interface IFlightService
    {
        [OperationContract]
        Task<string> SearchFlights(string searchTerm);
    }
}
