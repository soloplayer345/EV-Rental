using EV_Rental.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessLayer.Interfaces;
using BusinessLayer.DTOs;
using BusinessLayer.Services;

namespace EV_Rental.Pages.Admin
{
    public class DashboardModel : PageModel
    {
        private readonly IDashboardService _dashboardService;
        public DashboardDto? Dashboard { get; set; }

        public DashboardModel(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Kiểm tra quyền Admin
            if (!SessionHelper.IsAdmin(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            Dashboard = await _dashboardService.GetDashboardAsync();
            return Page();
        }
    }
}

