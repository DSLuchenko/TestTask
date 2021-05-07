using Server.Models.Entities;

namespace Server.Tools.DTO.Response
{
    public class SuccessRemoveUserJson
    {
        public string Msg { get; set; }
        public bool Success { get; set; }
        public User User { get; set; }
    }
}
