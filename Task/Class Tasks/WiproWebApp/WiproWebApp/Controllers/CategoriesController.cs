using Microsoft.AspNetCore.Mvc;
using WiproWebApp.Models;
using WiproWebApp.Data;

namespace WiproWebApp.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly FoodDeliveryContext _context;

        public CategoriesController(FoodDeliveryContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<Category> categories = _context.Categories.ToList();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category c)
        {
            _context.Categories.Add(c);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}