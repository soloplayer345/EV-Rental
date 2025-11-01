using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessLayer.Services;
using BusinessLayer.DTOs;
using DataAccessLayer.Entities;
using DataAccessLayer.Enums;
using EV_Rental.Helpers;

namespace EV_Rental.Pages.Renter
{
    public class ConfirmRentalModel : PageModel
    {
        private readonly VehicleService _vehicleService;
        private readonly RentalService _rentalService;

        public ConfirmRentalModel(VehicleService vehicleService, RentalService rentalService)
        {
            _vehicleService = vehicleService;
            _rentalService = rentalService;
        }

        public Vehicle? Vehicle { get; set; }
        public int VehicleId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PickupStationId { get; set; }
        public int ReturnStationId { get; set; }
        public string? Notes { get; set; }
        
        public string PickupStationName { get; set; } = string.Empty;
        public string ReturnStationName { get; set; } = string.Empty;
        public string RentalDuration { get; set; } = string.Empty;
        
        // Cost breakdown
        public decimal BasePrice { get; set; }
        public decimal ReservationFee { get; set; }
        public decimal DepositAmount { get; set; } = 3000000; // 3 triệu VNĐ
        public decimal TotalCost { get; set; }
        
        // Additional info
        public bool HasReservationFee { get; set; }
        public bool IsHourlyRental { get; set; }
        public int RentalDays { get; set; }
        public int RentalHours { get; set; }
        public int DaysUntilPickup { get; set; }
        
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(
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

            // Store data
            VehicleId = vehicleId;
            StartDate = startDate;
            EndDate = endDate;
            PickupStationId = pickupStationId;
            ReturnStationId = returnStationId;
            Notes = notes;

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
                return Page();
            }

            // Load station names
            var stations = await _rentalService.GetAllStationsAsync();
            PickupStationName = stations.FirstOrDefault(s => s.Id == pickupStationId)?.Name ?? "N/A";
            ReturnStationName = stations.FirstOrDefault(s => s.Id == returnStationId)?.Name ?? "N/A";

            // Calculate rental duration
            TimeSpan duration = endDate - startDate;
            if (duration.TotalDays >= 1)
            {
                RentalDays = (int)Math.Ceiling(duration.TotalDays);
                RentalDuration = $"{RentalDays} ngày";
                IsHourlyRental = false;
            }
            else
            {
                RentalHours = (int)Math.Ceiling(duration.TotalHours);
                RentalDuration = $"{RentalHours} giờ";
                IsHourlyRental = true;
            }

            // Calculate base price
            if (IsHourlyRental)
            {
                BasePrice = RentalHours * Vehicle.PricePerHour;
            }
            else
            {
                BasePrice = RentalDays * Vehicle.PricePerDay;
            }

            // Calculate days until pickup
            TimeSpan untilPickup = startDate - DateTime.Now;
            DaysUntilPickup = (int)Math.Ceiling(untilPickup.TotalDays);

            // Calculate reservation fee (if booking more than 1 day in advance)
            HasReservationFee = DaysUntilPickup > 1;
            if (HasReservationFee)
            {
                // Reservation fee = 10% of base price, minimum 50,000 VND
                ReservationFee = Math.Max(BasePrice * 0.1m, 50000);
            }
            else
            {
                ReservationFee = 0;
            }

            // Calculate total cost
            TotalCost = BasePrice + ReservationFee + DepositAmount;

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

            // Reload data for display in case of error
            VehicleId = vehicleId;
            StartDate = startDate;
            EndDate = endDate;
            PickupStationId = pickupStationId;
            ReturnStationId = returnStationId;
            Notes = notes;

            Vehicle = await _vehicleService.GetVehicleByIdAsync(vehicleId);
            
            if (Vehicle == null)
            {
                return RedirectToPage("/Renter/Index");
            }

            // Recalculate all values
            var stations = await _rentalService.GetAllStationsAsync();
            PickupStationName = stations.FirstOrDefault(s => s.Id == pickupStationId)?.Name ?? "N/A";
            ReturnStationName = stations.FirstOrDefault(s => s.Id == returnStationId)?.Name ?? "N/A";

            TimeSpan duration = endDate - startDate;
            if (duration.TotalDays >= 1)
            {
                RentalDays = (int)Math.Ceiling(duration.TotalDays);
                RentalDuration = $"{RentalDays} ngày";
                IsHourlyRental = false;
                BasePrice = RentalDays * Vehicle.PricePerDay;
            }
            else
            {
                RentalHours = (int)Math.Ceiling(duration.TotalHours);
                RentalDuration = $"{RentalHours} giờ";
                IsHourlyRental = true;
                BasePrice = RentalHours * Vehicle.PricePerHour;
            }

            TimeSpan untilPickup = startDate - DateTime.Now;
            DaysUntilPickup = (int)Math.Ceiling(untilPickup.TotalDays);
            HasReservationFee = DaysUntilPickup > 1;
            
            if (HasReservationFee)
            {
                ReservationFee = Math.Max(BasePrice * 0.1m, 50000);
            }
            else
            {
                ReservationFee = 0;
            }

            TotalCost = BasePrice + ReservationFee + DepositAmount;

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
                TempData["SuccessMessage"] = $"Đặt xe thành công! Tổng chi phí: {TotalCost:N0} đ. Vui lòng đến trạm nhận xe đúng giờ và thanh toán.";
                return RedirectToPage("/Renter/Index");
            }

            ErrorMessage = result.Message;
            return Page();
        }
    }
}
