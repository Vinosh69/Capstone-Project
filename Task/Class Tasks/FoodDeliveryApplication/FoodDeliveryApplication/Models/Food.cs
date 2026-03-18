namespace FoodDeliveryApplication.Models
{
    public class Food
    {
        public int Id { get; set; }
        public int CategId { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }

        public Category? Category { get; set; }
    }
}