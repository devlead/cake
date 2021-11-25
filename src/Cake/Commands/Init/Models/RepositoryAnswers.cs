using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Cake.Commands.Init.Models
{
    public class RepositoryAnswers
    {
        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("values")]
        public IDictionary<string, object> Values { get; set; }
    }
}