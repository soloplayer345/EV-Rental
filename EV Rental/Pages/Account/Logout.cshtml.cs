using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EV_Rental.Helpers;

namespace EV_Rental.Pages.Account
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Xóa session
            SessionHelper.ClearSession(HttpContext.Session);

            // Redirect về trang login với thông báo
            TempData["SuccessMessage"] = "Bạn đã đăng xuất thành công!";
            return RedirectToPage("/Account/Login");
        }
    }
}
