using BusinessLayer.DTOs;
using BusinessLayer.Services;
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

        public IEnumerable<VehicleDto> Vehicles { get; set; } = new List<VehicleDto>();

        public async Task OnGetAsync()
        {
            if (Status.HasValue)
            {
                // G?i hàm search trong service (l?c theo status)
                Vehicles = await _vehicleService.SearchVehiclesAsync(Name ?? "", Brand ?? "", Status.Value);
            }
            else
            {
                // N?u không ch?n status ? l?y toàn b?
                Vehicles = await _vehicleService.GetVehiclesAsync();
            }
        }
    }
}

