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
        public string Origin { get; set; }

        [XmlElement("destination")]
        public string Destination { get; set; }

        [XmlElement("departure")]
        public DateTime Departure { get; set; }

        [XmlElement("arrival")]
        public DateTime Arrival { get; set; }

        [XmlElement("price")]
        public decimal Price { get; set; }
    }
}
