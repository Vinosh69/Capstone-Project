using FoodDeliveryApp.Models;


namespace FoodDeliveryApplication.Models.ViewModels
{
    public class HomeViewModel
    {
        internal List<Cart> CartItems;

        public List<Category> Categories { get; set; }

        public List<Food> Foods { get; set; }

        public List<Cart> Cartitems { get; set; }

        public List<Sale> Sales { get; set; }
    }
}