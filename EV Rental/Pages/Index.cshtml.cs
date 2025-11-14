using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.Enums;
using DataAccessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EV_Rental.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IVehicleService _vehicleService;


        public IndexModel(IVehicleService vehicleService)
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
                Vehicles = await _vehicleService.SearchVehiclesAsync(Name ?? string.Empty, Brand ?? string.Empty, Status.Value);
            }
            else
            {
                Vehicles = await _vehicleService.GetVehiclesAsync();
            }
        }
    }
}

