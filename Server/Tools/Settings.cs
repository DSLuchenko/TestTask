using Microsoft.Extensions.Configuration;

namespace Server.Tools
{
    public class Settings
    {
        private readonly IConfiguration configuration;
        public Settings(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string MySqlConnectionStrings => configuration.GetConnectionString("MySQL");
        public int RunInterval => configuration.GetValue<int>("ReloadDataService:RunInterval");
        public string AuthName => configuration.GetValue<string>("BasicAuthParameters:Name");
        public string AuthPassword => configuration.GetValue<string>("BasicAuthParameters:Password");

    }
}
