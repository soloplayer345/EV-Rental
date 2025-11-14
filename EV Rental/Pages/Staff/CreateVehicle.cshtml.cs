using BusinessLayer.DTOs;
using BusinessLayer.Services;
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
        public VehicleCreateDto NewVehicle { get; set; } = new();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            NewVehicle.Status = VehicleStatus.Available;

            await _vehicleService.AddVehicleAsync(NewVehicle);
            return RedirectToPage("Index");
        }
    }
}

