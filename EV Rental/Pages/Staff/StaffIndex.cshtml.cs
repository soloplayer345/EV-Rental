using BusinessLayer.Services;
using DataAccessLayer.Entities;
using DataAccessLayer.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EV_Rental.Pages.Staff
{
    public class StaffIndexModel : PageModel
    {
        private readonly VehicleService _vehicleService;
        
        public StaffIndexModel(VehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        [BindProperty(SupportsGet = true)]
        public string Name { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Brand { get; set; }

        [BindProperty(SupportsGet = true)]
        public VehicleStatus? Status { get; set; }

        public IEnumerable<Vehicle> Vehicles { get; set; } = new List<Vehicle>();

        public async Task OnGetAsync()
        {
            if (Status.HasValue)
            {
                // Gọi hàm search trong service (lọc theo status)
                Vehicles = await _vehicleService.SearchVehiclesAsync(Name ?? "", Brand ?? "", Status.Value);
            }
            else
            {
                // Nếu không chọn status → lấy toàn bộ
                Vehicles = await _vehicleService.GetVehiclesAsync();
            }
        }
    }
}
