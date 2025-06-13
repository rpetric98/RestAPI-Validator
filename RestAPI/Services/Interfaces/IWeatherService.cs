using CookComputing.XmlRpc;

namespace RestAPI.Services.Interfaces
{
    public interface IWeatherService
    {
        [XmlRpcMethod("GetTemprature")]
        double getTemperature(string city);
        List<string> GetCities();
    }
}
