using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationClassLibrary
{
    class Response
    {
        public string Msg { get; set; }
        public string Success { get; set; }
        public User User { get; set; }
    }
}
