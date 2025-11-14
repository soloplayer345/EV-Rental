using BusinessLayer.Services;
using DataAccessLayer.Enums;
using EV_Rental.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EV_Rental.Pages.Api
{
    public class SearchVehiclesModel : PageModel
    {
        private readonly VehicleService _vehicleService;

        public SearchVehiclesModel(VehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        public async Task<IActionResult> OnGetAsync(string? name, string? brand, string? status)
        {
            if (!SessionHelper.IsRenter(HttpContext.Session))
            {
                return Unauthorized();
            }

            try
            {
                var vehicles = await _vehicleService.GetVehiclesAsync();
                var filteredVehicles = vehicles.Where(v =>
                    (string.IsNullOrWhiteSpace(name) || v.Name.Contains(name, StringComparison.OrdinalIgnoreCase)) &&
                    (string.IsNullOrWhiteSpace(brand) || v.Brand.Contains(brand, StringComparison.OrdinalIgnoreCase)) &&
                    (string.IsNullOrWhiteSpace(status) || v.Status.ToString() == status)
                ).Select(v => new
                {
                    v.Id,
                    v.Name,
                    v.Brand,
                    v.VehicleType,
                    v.PricePerDay,
                    v.MaxDistance,
                    v.BatteryCapacity,
                    v.seartCapacity,
                    Status = v.Status.ToString(),
                    v.ImageUrl
                });

                return new JsonResult(new
                {
                    success = true,
                    total = filteredVehicles.Count(),
                    data = filteredVehicles
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}
