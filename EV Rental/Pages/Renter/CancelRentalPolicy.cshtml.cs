using BusinessLayer.DTOs;
using BusinessLayer.Services;
using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces;
using EV_Rental.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EV_Rental.Pages.Renter
{
    public class CancelRentalPolicyModel : PageModel
    {
        private readonly RentalService _rentalService;
        private readonly VehicleService _vehicleService;

        public CancelRentalPolicyModel(IUnitOfWork unitOfWork, IVehicleRepo vehicleRepo)
        {
            _vehicleService = new VehicleService(vehicleRepo, unitOfWork);
            _rentalService = new RentalService(unitOfWork, _vehicleService);
        }

        public RentalRecord? Rental { get; set; }
        public Vehicle? Vehicle { get; set; }
        public CancellationPolicyDto? Policy { get; set; }

        public async Task<IActionResult> OnGetAsync(int rentalId)
        {
            // Kiểm tra quyền Renter
            if (!SessionHelper.IsRenter(HttpContext.Session))
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập trang này.";
                return RedirectToPage("/Account/Login");
            }

            // Lấy thông tin user từ session
            var user = SessionHelper.GetUserSession(HttpContext.Session);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin người dùng.";
                return RedirectToPage("/Account/Login");
            }

            int userId = user.AccountId;

            // Gọi RentalService.GetCancellationPolicyAsync()
            var result = await _rentalService.GetCancellationPolicyAsync(rentalId, userId);
            
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToPage("/Renter/MyTrips");
            }

            Policy = result.Data;

            // Lấy thông tin rental và vehicle
            Rental = await _rentalService.GetRentalByIdAsync(rentalId);
            if (Rental == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn thuê.";
                return RedirectToPage("/Renter/MyTrips");
            }

            Vehicle = await _vehicleService.GetVehicleByIdAsync(Rental.VehicleId);
            if (Vehicle == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin xe.";
                return RedirectToPage("/Renter/MyTrips");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int rentalId, bool agreedToPolicy)
        {
            // Kiểm tra quyền Renter
            if (!SessionHelper.IsRenter(HttpContext.Session))
            {
                TempData["ErrorMessage"] = "Bạn không có quyền thực hiện hành động này.";
                return RedirectToPage("/Account/Login");
            }

            // Lấy thông tin user từ session
            var user = SessionHelper.GetUserSession(HttpContext.Session);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin người dùng.";
                return RedirectToPage("/Account/Login");
            }

            int userId = user.AccountId;

            // Validate checkbox
            if (!agreedToPolicy)
            {
                TempData["ErrorMessage"] = "Bạn phải đồng ý với chính sách hủy đơn.";
                return RedirectToPage("/Renter/CancelRentalPolicy", new { rentalId });
            }

            // Gọi RentalService.CancelRentalWithPolicyAsync()
            var result = await _rentalService.CancelRentalWithPolicyAsync(rentalId, userId, agreedToPolicy);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToPage("/Renter/MyTrips");
        }
    }
}
