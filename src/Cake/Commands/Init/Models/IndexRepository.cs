using System.Text.Json.Serialization;

namespace Cake.Commands.Init.Models
{
    public class IndexRepository
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}