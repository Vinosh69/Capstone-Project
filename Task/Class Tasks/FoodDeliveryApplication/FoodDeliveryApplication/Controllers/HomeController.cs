using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodDeliveryApplication.Data;
using FoodDeliveryApplication.Models.ViewModels;

namespace FoodDeliveryApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var viewModel = new HomeViewModel
            {
                Categories = _context.Categories.ToList(),

                Foods = _context.Foods
                                .Include(f => f.Category)
                                .ToList(),

                CartItems = _context.Carts
                                    .Include(c => c.Food)
                                    .ToList(),

                Sales = _context.Sales
                                .Include(s => s.Food)
                                .ToList()
            };

            return View(viewModel);
        }
    }
}