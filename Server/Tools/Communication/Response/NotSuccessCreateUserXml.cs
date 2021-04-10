using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Server.Tools.Communication.Response
{
    [XmlRoot(ElementName = "Response")]
    [DataContract(Namespace = "")]
    public class NotSuccessCreateUserXml
    {
        [XmlAttribute("Status")]
        public string Status { get; set; }
        [XmlAttribute("ErrId")]
        public string ErrId { get; set; }
        [XmlElement("ErrorMsg")]
        public string ErrorMsg { get; set; }
    }
}
