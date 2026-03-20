

namespace FoodDeliveryApplication.Models
{
    public class Cart
    {
        public int Id { get; set; }

        public int FoodId { get; set; }

        public int Qty { get; set; }

        public decimal Price { get; set; }

        public decimal TotalAmount { get; set; }

        public int CustomerId { get; set; }

        public Food ? Food { get; set; } = null!;
    }
}