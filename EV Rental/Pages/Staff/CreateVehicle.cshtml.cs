using BusinessLayer.Services;
using DataAccessLayer.Entities;
using DataAccessLayer.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EV_Rental.Pages.Staff
{
    public class CreateVehicleModel : PageModel
    {
        private readonly VehicleService _vehicleService;

        public CreateVehicleModel(VehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        [BindProperty]
        public Vehicle Vehicle { get; set; } = new();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            Vehicle.Status = VehicleStatus.Available;

            await _vehicleService.AddVehicleAsync(Vehicle);
            return RedirectToPage("Index");
        }
    }
}
