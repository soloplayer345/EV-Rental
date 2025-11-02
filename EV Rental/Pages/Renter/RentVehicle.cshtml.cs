using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessLayer.Services;
using BusinessLayer.DTOs;
using DataAccessLayer.Entities;
using DataAccessLayer.Enums;
using EV_Rental.Helpers;

namespace EV_Rental.Pages.Renter
{
    public class RentVehicleModel : PageModel
    {
        private readonly VehicleService _vehicleService;
        private readonly RentalService _rentalService;

        public RentVehicleModel(VehicleService vehicleService, RentalService rentalService)
        {
            _vehicleService = vehicleService;
            _rentalService = rentalService;
        }

        public Vehicle? Vehicle { get; set; }
        public List<StationDto> AllStations { get; set; } = new List<StationDto>();
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int vehicleId)
        {
            // Check if user is logged in
            var accountId = SessionHelper.GetAccountId(HttpContext);
            if (accountId == null)
            {
                return RedirectToPage("/Account/Login");
            }

            // Load vehicle
            Vehicle = await _vehicleService.GetVehicleByIdAsync(vehicleId);
            
            if (Vehicle == null)
            {
                return RedirectToPage("/Renter/Index");
            }

            // Check if vehicle is available
            if (Vehicle.Status != VehicleStatus.Available)
            {
                ErrorMessage = "Xe này hiện không khả dụng để thuê.";
            }

            // Load all stations
            AllStations = await _rentalService.GetAllStationsAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(
            int vehicleId,
            DateTime startDate,
            DateTime endDate,
            int pickupStationId,
            int returnStationId,
            string? notes)
        {
            // Check if user is logged in
            var accountId = SessionHelper.GetAccountId(HttpContext);
            if (accountId == null)
            {
                return RedirectToPage("/Account/Login");
            }

            // Reload vehicle and stations for display
            Vehicle = await _vehicleService.GetVehicleByIdAsync(vehicleId);
            AllStations = await _rentalService.GetAllStationsAsync();

            // Validate dates
            if (startDate < DateTime.Now)
            {
                ErrorMessage = "Thời gian bắt đầu phải lớn hơn thời gian hiện tại.";
                return Page();
            }

            if (endDate <= startDate)
            {
                ErrorMessage = "Thời gian kết thúc phải lớn hơn thời gian bắt đầu.";
                return Page();
            }

            // Redirect to confirmation page with all parameters
            return RedirectToPage("/Renter/ConfirmRental", new
            {
                vehicleId = vehicleId,
                startDate = startDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                endDate = endDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                pickupStationId = pickupStationId,
                returnStationId = returnStationId,
                notes = notes
            });
        }
    }
}
