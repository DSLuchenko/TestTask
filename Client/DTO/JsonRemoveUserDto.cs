using Newtonsoft.Json;

namespace Client.DTO
{
    [JsonObject]
    class JsonRemoveUserDto
    {   
        [JsonProperty]
        public RemoveUser RemoveUser { get; set; }
    }

    class RemoveUser
    {
        public int Id { get; set; }
    }
}
