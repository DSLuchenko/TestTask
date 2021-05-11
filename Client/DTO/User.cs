using System.Xml.Serialization;

namespace Client.DTO
{
    public class User
    {
        [XmlAttribute]
        public int Id { get; set; }
        [XmlAttribute]
        public string Name { get; set; }
        [XmlElement]
        public string Status { get; set; }
    }
}
