using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EV_Rental.Pages.Staff
{
    public class StaffIndexModel : PageModel
    {
        private readonly IVehicleService _vehicleService;
        
        public StaffIndexModel(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        [BindProperty(SupportsGet = true)]
        public string Name { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Brand { get; set; }

        [BindProperty(SupportsGet = true)]
        public VehicleStatus? Status { get; set; }

        public IEnumerable<VehicleDto> Vehicles { get; set; } = new List<VehicleDto>();

        public async Task OnGetAsync()
        {
            if (Status.HasValue)
            {
                // search trong service (lọc theo status)
                Vehicles = await _vehicleService.SearchVehiclesAsync(Name ?? "", Brand ?? "", Status.Value);
            }
            else
            {
                // Nếu không chọn status sẽ lấy toàn bộ
                Vehicles = await _vehicleService.GetVehiclesAsync();
            }
        }
    }
}

