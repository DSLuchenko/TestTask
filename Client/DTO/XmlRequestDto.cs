using System.Xml.Serialization;

namespace Client.DTO
{
    [XmlRoot("Request")]
    public class XmlRequestDto
    {
        [XmlElement("user")]
        public User User { get; set; }
    }
}
