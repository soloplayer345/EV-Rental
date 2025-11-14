using BusinessLayer.Services;
using DataAccessLayer.Entities;
using DataAccessLayer.Enums;
using EV_Rental.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EV_Rental.Pages.Staff
{
    public class PickupVehicleModel : PageModel
    {
        private readonly RentalService _rentalService;
        private readonly VehicleService _vehicleService;

        public PickupVehicleModel(
            RentalService rentalService,
            VehicleService vehicleService)
        {
            _rentalService = rentalService;
            _vehicleService = vehicleService;
        }

        public List<DataAccessLayer.Entities.RentalRecord> ConfirmedRentals { get; set; } = new List<DataAccessLayer.Entities.RentalRecord>();
        public DataAccessLayer.Entities.RentalRecord? VerifiedRental { get; set; }
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
        
        [BindProperty]
        public string? OtpCode { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Kiểm tra quyền Staff
            if (!SessionHelper.IsStaff(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            // Lấy tất cả đơn chờ nhận (Confirmed status)
            // Lấy tất cả stations và đơn thuê của từng trạm
            var allStations = await _rentalService.GetAllStationsAsync();
            var allConfirmedRentals = new List<DataAccessLayer.Entities.RentalRecord>();

            foreach (var station in allStations)
            {
                var rentals = await _rentalService.GetConfirmedRentalsByStationAsync(station.Id);
                allConfirmedRentals.AddRange(rentals);
            }

            ConfirmedRentals = allConfirmedRentals.OrderByDescending(r => r.CreateDate).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostVerifyOtpAsync()
        {
            // Kiểm tra quyền Staff
            if (!SessionHelper.IsStaff(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            if (string.IsNullOrWhiteSpace(OtpCode))
            {
                ErrorMessage = "Vui lòng nhập mã OTP.";
                await LoadConfirmedRentals();
                return Page();
            }

            // Tìm đơn thuê theo OTP
            var rental = await _rentalService.GetRentalByOtpCodeAsync(OtpCode);

            if (rental == null)
            {
                ErrorMessage = "Mã OTP không hợp lệ hoặc đơn thuê không tồn tại.";
                await LoadConfirmedRentals();
                return Page();
            }

            // Hiển thị thông tin đơn thuê đã xác minh
            VerifiedRental = rental;
            await LoadConfirmedRentals();

            return Page();
        }

        public async Task<IActionResult> OnPostConfirmPickupAsync(string otpCode)
        {
            // Kiểm tra quyền Staff
            if (!SessionHelper.IsStaff(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            if (string.IsNullOrWhiteSpace(otpCode))
            {
                ErrorMessage = "Mã OTP không được để trống.";
                await LoadConfirmedRentals();
                return Page();
            }

            // Xác minh OTP và cập nhật trạng thái
            var result = await _rentalService.VerifyOtpAndPickupVehicleAsync(otpCode);

            if (!result.Success)
            {
                ErrorMessage = result.Message;
                await LoadConfirmedRentals();
                return Page();
            }

            SuccessMessage = $"Xác nhận nhận xe thành công! Đơn thuê #{result.Data?.Id} đã được cập nhật.";
            OtpCode = null;
            VerifiedRental = null;
            await LoadConfirmedRentals();

            return Page();
        }

        private async Task LoadConfirmedRentals()
        {
            var allStations = await _rentalService.GetAllStationsAsync();
            var allConfirmedRentals = new List<DataAccessLayer.Entities.RentalRecord>();

            foreach (var station in allStations)
            {
                var rentals = await _rentalService.GetConfirmedRentalsByStationAsync(station.Id);
                allConfirmedRentals.AddRange(rentals);
            }

            ConfirmedRentals = allConfirmedRentals.OrderByDescending(r => r.CreateDate).ToList();
        }
    }
}

