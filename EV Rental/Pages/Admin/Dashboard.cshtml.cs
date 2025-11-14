using EV_Rental.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessLayer.Interfaces;

namespace EV_Rental.Pages.Admin
{
    public class DashboardModel : PageModel
    {
        private readonly IReportService _reportService;
        
        public string UserEmail { get; set; } = string.Empty;
        public int NewReportsCount { get; set; } = 0;

        public DashboardModel(IReportService reportService)
        {
            _reportService = reportService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Kiểm tra quyền Admin
            if (!SessionHelper.IsAdmin(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            var user = SessionHelper.GetUserSession(HttpContext.Session);
            UserEmail = user?.Email ?? "";

            // Lấy số báo cáo mới
            NewReportsCount = await _reportService.GetNewReportsCountAsync();

            return Page();
        }
    }
}

