namespace WebApi.Options
{
    public class CorsOptions
    {
        public string[] AllowedOrigins { get; set; } = [];

        public string[] AllowedHeaders { get; set; } = [];

        public string[] AllowedMethods { get; set; } = [];
    }
}
