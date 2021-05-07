using System.Xml.Serialization;
using Server.Models.Entities;

namespace Server.Tools.DTO.Response
{
    [XmlRoot(ElementName = "Response")]
    public class SuccessCreateUserXml
    {
        [XmlElement(ElementName = "user")]
        public User User { get; set; }
        [XmlAttribute("Status")]
        public string Status { get; set; }
        [XmlAttribute("ErrId")]
        public string ErrId { get; set; }
    }
}
