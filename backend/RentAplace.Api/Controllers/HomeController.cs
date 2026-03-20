using Microsoft.AspNetCore.Mvc;

namespace RentAPlace.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}