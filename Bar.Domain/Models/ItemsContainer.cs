using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Bar.Domain.Models
{
    public class ItemsContainer<T>
    {
        [JsonPropertyName("$values")]
        public List<T> Values { get; set; } = new List<T>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
