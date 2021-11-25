using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Cake.Commands.Init.Models
{
    public class RepositoryQuestion
    {
        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("answers")]
        public ICollection<RepositoryAnswers> Answers { get; set; }
    }
}