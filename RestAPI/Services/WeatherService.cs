using RestAPI.Services.Interfaces;
using System.Xml;

namespace RestAPI.Services
{
    public class WeatherService : IWeatherService
    {
        public async Task<List<string>> GetCitiesAsync()
        {
            string url = "https://vrijeme.hr/hrvatska_n.xml";
            using HttpClient client = new();
            var response = await client.GetStringAsync(url);

            var xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.LoadXml(response);
            }
            catch (XmlException ex)
            {
                throw new Exception("Invalid XML format: " + ex.Message);
            }

            var cityNodes = xmlDoc.SelectNodes("//Grad/GradIme");
            var cities = new HashSet<string>();

            foreach (XmlNode node in cityNodes)
            {
                if (!string.IsNullOrWhiteSpace(node.InnerText))
                    cities.Add(node.InnerText.Trim());
            }

            return cities.OrderBy(x => x).ToList();
        }

        public double getTemperature(string city)
        {
            var temperature = FethTemperature(city);
            return temperature;
        }

        private double FethTemperature(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                return double.NaN; // Or some default value indicating no data

            string url = "https://vrijeme.hr/hrvatska_n.xml";
            using (HttpClient client = new())
            {
                var response = client.GetStringAsync(url).Result;
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(response);

                // Normalize city for comparison (lowercase trimmed)
                var normalizedCity = city.Trim().ToLower();

                // Use XPath with translate to do case-insensitive search
                var cityNode = xmlDoc.SelectSingleNode(
                    $"//Grad[translate(normalize-space(GradIme), 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') = '{normalizedCity}']"
                );

                var tempNode = cityNode?.SelectSingleNode("Podatci/Temp");

                if (tempNode != null && double.TryParse(tempNode.InnerText, out double temperature))
                    return temperature;
            }

            // Return NaN if not found instead of throwing
            return double.NaN;
        }
    }
}
