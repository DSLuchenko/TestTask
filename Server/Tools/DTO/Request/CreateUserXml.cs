using System.Xml.Serialization;
using Server.Models.Entities;

namespace Server.Tools.DTO.Request
{
    [XmlRoot(ElementName = "Request")]
    public class CreateUserXml
    {
        [XmlElement(ElementName = "user")]
        public User User { get; set; }
    }
}
