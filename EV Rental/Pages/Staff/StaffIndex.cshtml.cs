using BusinessLayer.DTOs;
using BusinessLayer.Services;
using DataAccessLayer.Entities;
using DataAccessLayer.Enums;
using DataAccessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EV_Rental.Pages.Staff
{
    public class StaffIndexModel : PageModel
    {
        private readonly IVehicleRepo _vehicleRepo;
        private readonly VehicleService _vehicleService;
        public StaffIndexModel(IVehicleRepo vehicleRepo, VehicleService vehicleService)
        {
            _vehicleRepo = vehicleRepo;
            _vehicleService = vehicleService;
        }

        [BindProperty(SupportsGet = true)]
        public string Name { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Brand { get; set; }

        [BindProperty(SupportsGet = true)]
        public VehicleStatus? Status { get; set; }

        public IEnumerable<VehicleDto> Vehicles { get; set; }

        public async Task OnGetAsync()
        {
            if (Status.HasValue)
            {
                // Gọi hàm search trong service (lọc theo status)
                Vehicles = (IEnumerable<VehicleDto>)await _vehicleService.SearchVehiclesAsync(Name, Brand, Status.Value);
            }
            else
            {
                // Nếu không chọn status → lấy toàn bộ
                Vehicles = (IEnumerable<VehicleDto>)await _vehicleService.GetVehiclesAsync();
            }
        }
    }
}
