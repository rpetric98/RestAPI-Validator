using RestAPI.Services.Interfaces;
using System.Xml;

namespace RestAPI.Services
{
    public class WeatherService : IWeatherService
    {
        public List<string> GetCities()
        {
            string url = "https://vrijeme.hr/hrvatska_n.xml";
            using (HttpClient client = new())
            {
                var response = client.GetStringAsync(url).Result;
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(response);

                var cityNodes = xmlDoc.SelectSingleNode($"//Grad/GradIme");
                var cities = new List<string>();
                foreach (XmlNode node in cityNodes)
                {
                    cities.Add(node.InnerText);
                }
                return cities;
            }
        }

        public double getTemperature(string city)
        {
            var temperature = FethTemperature(city);
            return temperature;
        }

        private double FethTemperature(string city)
        {
            string url = "https://vrijeme.hr/hrvatska_n.xml";
            using (HttpClient client = new())
            { 
                var response = client.GetStringAsync(url).Result;
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(response);

                var cityNode = xmlDoc.SelectSingleNode($"//Grad[GradIme='{city}']");
                if (cityNode != null)
                {
                    var tempNode = cityNode.SelectSingleNode("Podatci/Temp");
                    if (tempNode != null)
                    {
                        return double.Parse(tempNode.InnerText);
                    }
                }
            }
            throw new Exception("City not found or temperature data is missing.");
        }
    }
}
