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

            // Create rental request
            var request = new CreateRentalRequestDto
            {
                VehicleId = vehicleId,
                PickupStationId = pickupStationId,
                ReturnStationId = returnStationId,
                StartTime = startDate,
                ExpectedEndTime = endDate,
                Notes = notes
            };

            // Call rental service
            var result = await _rentalService.CreateRentalAsync(accountId.Value, request);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToPage("/Renter/Index");
            }

            ErrorMessage = result.Message;
            return Page();
        }
    }
}
