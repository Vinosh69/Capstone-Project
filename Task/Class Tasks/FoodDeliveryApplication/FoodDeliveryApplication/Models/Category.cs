using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryApplication.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public ICollection<Food> Foods { get; set; } = new List<Food>();
    }
}