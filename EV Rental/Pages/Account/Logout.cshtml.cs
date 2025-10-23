using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EV_Rental.Pages.Account
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Xóa session
            HttpContext.Session.Clear();

            // Redirect về trang login
            return RedirectToPage("/Account/Login");
        }
    }
}
