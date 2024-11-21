using Bar.Domain.Models;

namespace Bar.Domain.Entities
{
    public class Product
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImagePath { get; set; }
        public string? ImageMimeType { get; set; }

        // Используем только ID для сериализации
        public int? CategoryID { get; set; }

        // Связь с объектом Category
        public Category? Category { get; set; }
    }
}
