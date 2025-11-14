using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EV_Rental.Pages.Staff
{
    public class CreateVehicleModel : PageModel
    {
        private readonly IVehicleService _vehicleService;

        public CreateVehicleModel(IVehicleService vehicleService)
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

