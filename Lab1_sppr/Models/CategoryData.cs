using Bar.Domain.Entities;
using System.Text.Json.Serialization;

namespace Lab1_sppr.Models
{
    public class CategoryData
    {
        [JsonPropertyName("$values")] 
        public List<Category> Values { get; set; }
    }
}
