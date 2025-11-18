using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EV_Rental.Pages.Staff
{
    public class DeleteVehicleModel : PageModel
    {
        private readonly IVehicleService _vehicleService;

        public DeleteVehicleModel(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        [BindProperty]
        public VehicleDto Vehicle { get; set; } = new();

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

