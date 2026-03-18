namespace Gamism.SDK.Extensions.AspNetCore.Swagger
{
    public class SwaggerOptions
    {
        public bool Enabled { get; set; } = true;
        public string Title { get; set; } = "API Documentation";
        public string Description { get; set; } = "API Documentation";
        public string Version { get; set; } = "v1";
        public string Group { get; set; } = "default";
        public string[] PathsToMatch { get; set; } = ["/**"];
    }
}
