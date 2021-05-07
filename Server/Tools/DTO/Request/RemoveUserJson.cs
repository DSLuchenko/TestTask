using Newtonsoft.Json;

namespace Server.Tools.DTO.Request
{
    public class RemoveUserJson
    {
        [JsonProperty("RemoveUser")]
        public RemoveUser RemoveUser { get; set; }
    }

    public class RemoveUser
    {
        [JsonProperty("Id")]
        public int Id { get; set; }
    }
}
