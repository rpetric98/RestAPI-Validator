using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace RestAPI.Models
{
    [XmlRoot("flight")]
    public class FlightInfo
    {
        [Key]
        public int Id { get; set; }

        [XmlElement("origin")]
        public string Origin { get; set; } = string.Empty;

        [XmlElement("destination")]
        public string Destination { get; set; } = string.Empty;

        [XmlElement("departure")]
        public DateTime Departure { get; set; }

        [XmlElement("arrival")]
        public DateTime Arrival { get; set; }

        [XmlElement("price")]
        public decimal Price { get; set; }
    }
}
