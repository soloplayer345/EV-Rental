using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessLayer.Services;
using BusinessLayer.DTOs;

namespace EV_Rental.Pages.Renter
{
    public class VehicleDetailModel : PageModel
    {
        private readonly VehicleService _vehicleService;
        private readonly ReviewService _reviewService;

        public VehicleDetailModel(VehicleService vehicleService, ReviewService reviewService)
        {
            _vehicleService = vehicleService;
            _reviewService = reviewService;
        }

        public VehicleDto? Vehicle { get; set; }
        public IEnumerable<RatingReviewDto> Reviews { get; set; } = new List<RatingReviewDto>();
        public double AverageRating { get; set; } = 0;
        public int TotalReviews { get; set; } = 0;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                Vehicle = await _vehicleService.GetVehicleByIdAsync(id);
                
                if (Vehicle == null)
                {
                    return RedirectToPage("/Renter/Index");
                }
                
                // Lấy tất cả reviews
                var reviewsResult = await _reviewService.GetAllReviewsAsync();
                if (reviewsResult.Success && reviewsResult.Data != null)
                {
                    // Filter reviews cho vehicle này
                    Reviews = reviewsResult.Data.Where(r => r.VehicleId == id).OrderByDescending(r => r.CreatedAt);
                    
                    TotalReviews = Reviews.Count();
                    if (TotalReviews > 0)
                    {
                        AverageRating = Reviews.Average(r => r.Rating);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error nếu cần
                Reviews = new List<RatingReviewDto>();
            }
            
            return Page();
        }
    }
}

