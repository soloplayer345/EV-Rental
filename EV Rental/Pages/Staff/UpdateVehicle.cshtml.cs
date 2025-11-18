using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EV_Rental.Pages.Staff
{
    public class UpdateVehicleModel : PageModel
    {
        private readonly IVehicleService _vehicleService;

        public UpdateVehicleModel(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        [BindProperty]
        public VehicleUpdateDto? Vehicle { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var existing = await _vehicleService.GetVehicleByIdAsync(id);
            if (existing == null)
                return NotFound();

            // Map VehicleDto to VehicleUpdateDto
            Vehicle = new VehicleUpdateDto
            {
                Id = existing.Id,
                StationId = existing.StationId,
                Name = existing.Name,
                Brand = existing.Brand,
                PlateNumber = existing.PlateNumber,
                Model = existing.Model,
                VehicleType = existing.VehicleType,
                Status = existing.Status,
                PricePerHour = existing.PricePerHour,
                PricePerDay = existing.PricePerDay,
                Features = existing.Features,
                ImageUrl = existing.ImageUrl,
                MaxDistance = existing.MaxDistance,
                BatteryCapacity = existing.BatteryCapacity
            };
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


