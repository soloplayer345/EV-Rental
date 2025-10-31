using BusinessLayer.DTOs;
using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EV_Rental.Pages.Staff
{
    public class StaffIndexModel : PageModel
    {
        private readonly IVehicleRepo _vehicleRepo;
        public StaffIndexModel(IVehicleRepo vehicleRepo)
        {
            _vehicleRepo = vehicleRepo;
        }

        [BindProperty(SupportsGet = true)]
        public VehicleSearchDto Search { get; set; } = new();

        public IEnumerable<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        public object Vehicle { get; private set; }

        public async Task OnGetAsync()
        {
            Vehicle = await _vehicleRepo.SearchVehiclesAsync(
                Search.Name, Search.Brand, Search.VehicleType, Search.Status
                );
        }
    }
}
