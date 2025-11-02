using BusinessLayer.Services;
using DataAccessLayer.Entities;
using DataAccessLayer.Enums;
using EV_Rental.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EV_Rental.Pages.Renter
{
    public class IndexModel : PageModel
    {
        private readonly VehicleService _vehicleService;
        private const int PageSize = 6;

        public IndexModel(VehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        public string UserEmail { get; set; } = string.Empty;
        public List<Vehicle> AvailableVehicles { get; set; } = new List<Vehicle>();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int TotalVehicles { get; set; }

        public async Task<IActionResult> OnGetAsync(int pageNumber = 1)
        {
            // Kiểm tra quyền Renter
            if (!SessionHelper.IsRenter(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            var user = SessionHelper.GetUserSession(HttpContext.Session);
            UserEmail = user?.Email ?? "";

            // Lấy danh sách xe có sẵn
            var vehicles = await _vehicleService.GetVehiclesAsync();
            var availableVehicles = vehicles.Where(v => v.Status == VehicleStatus.Available).ToList();

            // Tính toán paging
            TotalVehicles = availableVehicles.Count;
            TotalPages = (int)Math.Ceiling(TotalVehicles / (double)PageSize);
            CurrentPage = pageNumber < 1 ? 1 : (pageNumber > TotalPages ? TotalPages : pageNumber);

            // Lấy xe cho trang hiện tại
            AvailableVehicles = availableVehicles
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            return Page();
        }
    }
}
