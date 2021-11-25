using System.Text.Json.Serialization;

namespace Cake.Commands.Init.Models
{
    public class Index
    {
        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("repositories")]
        public IndexRepository[] Repositories { get; set; }
    }
}