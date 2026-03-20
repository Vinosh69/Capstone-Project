using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using razorpage.Pages.Models;


namespace RazorPagesDemo.Pages
{
    public class AddItemModel : PageModel
    {
        [BindProperty]
        public string ItemName { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!string.IsNullOrEmpty(ItemName))
            {
                IndexModel.Items.Add(new item { Name = ItemName });
            }

            return RedirectToPage("Index");
        }
    }
}