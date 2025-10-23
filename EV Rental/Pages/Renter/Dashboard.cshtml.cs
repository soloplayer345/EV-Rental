using EV_Rental.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EV_Rental.Pages.Renter
{
    public class DashboardModel : PageModel
    {
        public string UserEmail { get; set; } = string.Empty;

        public IActionResult OnGet()
        {
            // Kiểm tra quyền Renter
            if (!SessionHelper.IsRenter(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            var user = SessionHelper.GetUserSession(HttpContext.Session);
            UserEmail = user?.Email ?? "";

            return Page();
        }
    }
}
