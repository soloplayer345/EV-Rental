using DataAccessLayer.Entities;
using DataAccessLayer.Enums;
using DataAccessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EV_Rental.Pages.Staff
{
    public class CreateVehicleModel : PageModel
    {
        private readonly IVehicleRepo _vehicleRepo;

        public CreateVehicleModel(IVehicleRepo vehicleRepo)
        {
            _vehicleRepo = vehicleRepo;
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

            await _vehicleRepo.AddAsync(Vehicle);
            return RedirectToPage("Index");
        }
    }
}
