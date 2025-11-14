using DataAccessLayer.Entities;

namespace DataAccessLayer.Interfaces
{
    public interface IRatingReviewRepo : IGenericRepo<RatingReview>
    {
        Task<IEnumerable<RatingReview>> GetAllWithDetailsAsync();
        Task<RatingReview?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<RatingReview>> GetByVehicleIdAsync(int vehicleId);
        Task<double> GetAverageRatingForVehicleAsync(int vehicleId);
        Task<Dictionary<int, int>> GetRatingDistributionAsync();
    }
}
