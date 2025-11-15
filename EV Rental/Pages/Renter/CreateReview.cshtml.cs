using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using EV_Rental.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EV_Rental.Pages.Renter
{
    public class CreateReviewModel : PageModel
    {
        private readonly IReviewService _reviewService;
        private readonly IRentalService _rentalService;
        private readonly IVehicleService _vehicleService;

        public CreateReviewModel(IReviewService reviewService, IRentalService rentalService, IVehicleService vehicleService)
        {
            _reviewService = reviewService;
            _rentalService = rentalService;
            _vehicleService = vehicleService;
        }

        [BindProperty]
        public CreateRatingReviewDto ReviewRequest { get; set; } = new();

        public RentalRecordDto? Rental { get; set; }
        public VehicleDto? Vehicle { get; set; }
        public RatingReviewDto? ExistingReview { get; set; }

        public async Task<IActionResult> OnGetAsync(int rentalId)
        {
            // Kiểm tra quyền Renter
            if (!SessionHelper.IsRenter(HttpContext.Session))
            {
                return RedirectToPage("/Auth/Login");
            }

            var user = SessionHelper.GetUserSession(HttpContext.Session);
            if (user == null)
            {
                return RedirectToPage("/Auth/Login");
            }

            // Lấy thông tin rental
            var rentalEntity = await _rentalService.GetRentalByIdAsync(rentalId);
            if (rentalEntity == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy chuyến đi";
                return RedirectToPage("/Renter/MyTrips");
            }

            Rental = BusinessLayer.Mapping.RentalRecordMapper.ToDto(rentalEntity);

            // Kiểm tra quyền sở hữu
            if (Rental.RenterId != user.AccountId)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền đánh giá chuyến đi này";
                return RedirectToPage("/Renter/MyTrips");
            }

            // Kiểm tra trạng thái
            if (Rental.Status != DataAccessLayer.Enums.RentalRecordStatus.Completed)
            {
                TempData["ErrorMessage"] = "Chỉ có thể đánh giá sau khi hoàn thành chuyến đi";
                return RedirectToPage("/Renter/MyTrips");
            }

            // Kiểm tra đã đánh giá chưa
            var existingReviewResult = await _reviewService.GetReviewByRentalIdAsync(rentalId);
            if (existingReviewResult.Success && existingReviewResult.Data != null)
            {
                ExistingReview = existingReviewResult.Data;
            }

            // Lấy thông tin xe
            Vehicle = await _vehicleService.GetVehicleByIdAsync(Rental.VehicleId);

            ReviewRequest.RentalId = rentalId;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Kiểm tra quyền Renter
            if (!SessionHelper.IsRenter(HttpContext.Session))
            {
                return RedirectToPage("/Auth/Login");
            }

            var user = SessionHelper.GetUserSession(HttpContext.Session);
            if (user == null)
            {
                return RedirectToPage("/Auth/Login");
            }

            if (!ModelState.IsValid)
            {
                // Reload data
                await OnGetAsync(ReviewRequest.RentalId);
                return Page();
            }

            // Tạo review
            var result = await _reviewService.CreateReviewAsync(ReviewRequest, user.AccountId);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToPage("/Renter/MyTrips");
            }

            ModelState.AddModelError(string.Empty, result.Message);
            // Reload data
            await OnGetAsync(ReviewRequest.RentalId);
            return Page();
        }
    }
}
