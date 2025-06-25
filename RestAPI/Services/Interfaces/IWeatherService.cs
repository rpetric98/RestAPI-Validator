using CookComputing.XmlRpc;

namespace RestAPI.Services.Interfaces
{
    public interface IWeatherService
    {
        [XmlRpcMethod("GetTemperature")]
        double getTemperature(string city);
        Task<List<string>> GetCitiesAsync();
    }
}
