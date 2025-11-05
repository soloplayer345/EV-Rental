using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EV_Rental.Pages.Staff
{
    public class UpdateVehicleModel : PageModel
    {
        private readonly IVehicleRepo _vehicleRepo;

        public UpdateVehicleModel(IVehicleRepo vehicleRepo)
        {
            _vehicleRepo = vehicleRepo;
        }

        [BindProperty]
        public Vehicle Vehicle { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var existing = await _vehicleRepo.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            Vehicle = existing;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            await _vehicleRepo.Update(Vehicle);
            return RedirectToPage("Index");
        }
    }
}
