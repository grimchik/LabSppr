using Bar.Domain.Entities;
using System.Text.Json.Serialization;

namespace Bar.Domain.Models
{
    public class CategoryData
    {
        [JsonPropertyName("data")]
        public List<Category> Data { get; set; }
    }
}
