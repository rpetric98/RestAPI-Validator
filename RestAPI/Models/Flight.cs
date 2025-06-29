using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Xml.Serialization;

namespace RestAPI.Models
{
        [XmlRoot("FlightDetails")]
        public class FlightDetails
        {
            [Key]
            [XmlIgnore]    
            public int Id { get; set; }

            [XmlArray("Legs")]
            [XmlArrayItem("Leg")]
            public List<Leg> Legs { get; set; } = new List<Leg>();

            [Required]
            public int Adults { get; set; }

            [Required]
            public string Currency { get; set; } = string.Empty;

            [Required]
            public string Locale { get; set; } = string.Empty;

            [Required]
            public string Market { get; set; } = string.Empty;

            [Required]
            public string CabinClass { get; set; } = string.Empty;

            [Required]
            public string CountryCode { get; set; } = string.Empty;
        }

        public class Leg
        {
            [Key]
            [XmlIgnore]
            public int Id { get; set; }

            [Required]
            public string Origin { get; set; } = string.Empty;

            [Required]
            public string Destination { get; set; } = string.Empty;

            [Required]
            public DateTime Date { get; set; }

             [XmlIgnore]
            public int FlightDetailsId { get; set; }
             [XmlIgnore]    
             public FlightDetails FlightDetails { get; set; } = null!;
        }
    }

