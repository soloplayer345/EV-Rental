using BusinessLayer.DTOs;
using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EV_Rental.Pages.Admin
{
    public class ReviewsModel : PageModel
    {
        private readonly ReviewService _reviewService;

        public ReviewsModel(ReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        public ReviewStatisticsDto Statistics { get; set; } = new();
        public IEnumerable<RatingReviewDto> Reviews { get; set; } = new List<RatingReviewDto>();
        public IEnumerable<RatingReviewDto> FilteredReviews { get; set; } = new List<RatingReviewDto>();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? RatingFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SortBy { get; set; } = "newest";

        public async Task<IActionResult> OnGetAsync()
        {
            // Get statistics
            var statsResult = await _reviewService.GetReviewStatisticsAsync();
            if (statsResult.Success && statsResult.Data != null)
            {
                Statistics = statsResult.Data;
            }

            // Get all reviews
            var reviewsResult = await _reviewService.GetAllReviewsAsync();
            if (!reviewsResult.Success || reviewsResult.Data == null)
            {
                TempData["ErrorMessage"] = reviewsResult.Message ?? "Không thể tải danh sách đánh giá";
                Reviews = new List<RatingReviewDto>();
                FilteredReviews = new List<RatingReviewDto>();
                return Page();
            }

            Reviews = reviewsResult.Data;

            // Apply filters
            FilteredReviews = Reviews;

            // Filter by search term
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                FilteredReviews = FilteredReviews.Where(r =>
                    r.VehicleName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    r.VehiclePlateNumber.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    r.RenterName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    r.RenterEmail.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (r.Comment ?? "").Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)
                );
            }

            // Filter by rating
            if (RatingFilter.HasValue)
            {
                FilteredReviews = FilteredReviews.Where(r => r.Rating == RatingFilter.Value);
            }

            // Sort
            FilteredReviews = SortBy switch
            {
                "oldest" => FilteredReviews.OrderBy(r => r.CreatedAt),
                "highest" => FilteredReviews.OrderByDescending(r => r.Rating).ThenByDescending(r => r.CreatedAt),
                "lowest" => FilteredReviews.OrderBy(r => r.Rating).ThenByDescending(r => r.CreatedAt),
                _ => FilteredReviews.OrderByDescending(r => r.CreatedAt) // newest
            };

            return Page();
        }
    }
}
