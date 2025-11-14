using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.Enums;
using EV_Rental.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EV_Rental.Pages.Renter
{
    public class IndexModel : PageModel
    {
        private readonly IVehicleService _vehicleService;
        private const int PageSize = 6;

        public IndexModel(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        public string UserEmail { get; set; } = string.Empty;
        public List<VehicleDto> AvailableVehicles { get; set; } = new List<VehicleDto>();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int TotalVehicles { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Name { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Brand { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Status { get; set; }

        public async Task<IActionResult> OnGetAsync(int pageNumber = 1)
        {
            // Kiểm tra quyền Renter
            if (!SessionHelper.IsRenter(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            var user = SessionHelper.GetUserSession(HttpContext.Session);
            UserEmail = user?.Email ?? "";

            // Lấy danh sách xe
            IEnumerable<VehicleDto> vehicles;

            // Áp dụng filter nếu có
            bool hasFilter = !string.IsNullOrWhiteSpace(Name) || 
                           !string.IsNullOrWhiteSpace(Brand) || 
                           !string.IsNullOrWhiteSpace(Status);

            if (hasFilter)
            {
                // Nếu có filter, sử dụng search
                VehicleStatus? statusEnum = null;
                if (!string.IsNullOrWhiteSpace(Status) && Enum.TryParse<VehicleStatus>(Status, out var parsedStatus))
                {
                    statusEnum = parsedStatus;
                }

                if (statusEnum.HasValue)
                {
                    vehicles = await _vehicleService.SearchVehiclesAsync(Name ?? "", Brand ?? "", statusEnum.Value);
                }
                else
                {
                    // Lấy tất cả xe và filter theo name và brand
                    var allVehicles = await _vehicleService.GetVehiclesAsync();
                    vehicles = allVehicles.Where(v => 
                        (string.IsNullOrWhiteSpace(Name) || v.Name.Contains(Name, StringComparison.OrdinalIgnoreCase)) &&
                        (string.IsNullOrWhiteSpace(Brand) || v.Brand.Contains(Brand, StringComparison.OrdinalIgnoreCase))
                    );
                }
            }
            else
            {
                // Không có filter, lấy tất cả xe
                vehicles = await _vehicleService.GetVehiclesAsync();
            }

            var vehicleList = vehicles.ToList();

            // Tính toán paging
            TotalVehicles = vehicleList.Count;
            TotalPages = (int)Math.Ceiling(TotalVehicles / (double)PageSize);
            CurrentPage = pageNumber < 1 ? 1 : (pageNumber > TotalPages && TotalPages > 0 ? TotalPages : pageNumber);

            // Lấy xe cho trang hiện tại
            AvailableVehicles = vehicleList
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            return Page();
        }
    }
}

