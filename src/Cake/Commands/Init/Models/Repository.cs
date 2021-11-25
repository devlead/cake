using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Cake.Commands.Init.Models
{
    public class Repository
    {
        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("templates")]
        public RepositoryTemplate[] Templates { get; set; }

        [JsonPropertyName("questions")]
        public ICollection<RepositoryQuestion> Questions { get; set; }
    }
}