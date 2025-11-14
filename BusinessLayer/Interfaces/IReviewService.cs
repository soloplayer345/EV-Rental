using BusinessLayer.DTOs;

namespace BusinessLayer.Interfaces
{
    public interface IReviewService
    {
        Task<ServiceResultDto<IEnumerable<RatingReviewDto>>> GetAllReviewsAsync();
        Task<ServiceResultDto<ReviewStatisticsDto>> GetReviewStatisticsAsync();
        Task<ServiceResultDto<IEnumerable<RatingReviewDto>>> GetReviewsByVehicleAsync(int vehicleId);
    }
}
