using FoodDeliveryApplication.Models;

public class Sale
{
    public int SaleId { get; set; }

    public int FoodId { get; set; }
    public Food ? Food { get; set; } = null!;

    public string UserId { get; set; } = string.Empty;

    public DateTime Date { get; set; }
    public string Status { get; set; } = string.Empty;

    public int Quantity { get; set; }
    public decimal TotalAmount { get; set; }
}