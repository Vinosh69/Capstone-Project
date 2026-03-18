using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryApplication.Models
{
    public class ProductsSold
    {
        [Key]
        public int Id { get; set; }   

        public int ProductId { get; set; }

        public int SaleId { get; set; }

        public int Qty { get; set; }

        public decimal TotalProductAmount { get; set; }

        public string Status { get; set; } = string.Empty;

        public Food ? Food { get; set; } = null!;

        public Sale ? Sale { get; set; } = null!;
    }
}