using Microsoft.AspNetCore.Mvc.RazorPages;
using razorpage.Pages.Models;
using System.Collections.Generic;

namespace RazorPagesDemo.Pages
{
    public class IndexModel : PageModel
    {
        public static List<item> Items = new List<item>();

        public void OnGet()
        {
        }
    }
}