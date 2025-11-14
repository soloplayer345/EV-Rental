using BusinessLayer.Services;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EV_Rental.Pages.Staff
{
    public class UpdateVehicleModel : PageModel
    {
        private readonly VehicleService _vehicleService;

        public UpdateVehicleModel(VehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        [BindProperty]
        public Vehicle Vehicle { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var existing = await _vehicleService.GetVehicleByIdAsync(id);
            if (existing == null)
                return NotFound();

            Vehicle = existing;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            await _vehicleService.UpdateVehicleAsync(Vehicle);
            return RedirectToPage("Index");
        }
    }
}
