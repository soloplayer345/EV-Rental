using BusinessLayer.DTOs;
using DataAccessLayer.Entities;
using DataAccessLayer.Enums;
using DataAccessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EV_Rental.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IVehicleRepo _vehicleRepo;

        public IndexModel(ILogger<IndexModel> logger, IVehicleRepo vehicleRepo)
        {
            _logger = logger;
            _vehicleRepo = vehicleRepo;
        }

        public IEnumerable<Vehicle> Vehicles { get; set; } = new List<Vehicle>();

        [BindProperty(SupportsGet = true)]
        public string? SearchName { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchBrand { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchType { get; set; }

        [BindProperty(SupportsGet = true)]
        public VehicleStatus? SearchStatus { get; set; }

        public async Task OnGetAsync()
        {
            if (string.IsNullOrEmpty(SearchName) && string.IsNullOrEmpty(SearchBrand) && string.IsNullOrEmpty(SearchType))
            {
                Vehicles = await _vehicleRepo.GetAllAsync();
            }
            else
            {
                Vehicles = await _vehicleRepo.SearchVehiclesAsync(SearchName, SearchBrand, SearchType, SearchStatus);
            }
        }
    }
}
