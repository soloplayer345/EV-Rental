using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessLayer.Services;
using DataAccessLayer.Entities;

namespace EV_Rental.Pages.Renter
{
    public class VehicleDetailModel : PageModel
    {
        private readonly VehicleService _vehicleService;

        public VehicleDetailModel(VehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        public Vehicle? Vehicle { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Vehicle = await _vehicleService.GetVehicleByIdAsync(id);
            
            if (Vehicle == null)
            {
                return RedirectToPage("/Renter/Index");
            }
            
            return Page();
        }
    }
}
