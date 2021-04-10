using System.Xml.Serialization;

namespace Server.Models.Entities
{
    public class User
    {
        [XmlAttribute("Id")]
        public int Id { get; set; }
        [XmlAttribute("Name")]
        public string Name { get; set; }
        [XmlElement("Status")]
        public string Status { get; set; }
    }
}
