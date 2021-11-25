using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Cake.Commands.Init.Models
{
    public class RepositoryTemplate
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("questions")]
        public ICollection<RepositoryQuestion> Questions { get; set; }
    }
}