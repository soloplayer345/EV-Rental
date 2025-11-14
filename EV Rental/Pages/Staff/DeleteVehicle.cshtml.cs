using BusinessLayer.Services;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EV_Rental.Pages.Staff
{
    public class DeleteVehicleModel : PageModel
    {
        private readonly VehicleService _vehicleService;

        public DeleteVehicleModel(VehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        [BindProperty]
        public Vehicle Vehicle { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Vehicle = await _vehicleService.GetVehicleByIdAsync(id);
            if (Vehicle == null) return RedirectToPage("Index");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _vehicleService.DeleteVehicleAsync(Vehicle.Id);
            return RedirectToPage("Index");
        }
    }
}
