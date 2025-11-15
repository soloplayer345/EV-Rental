using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories
{
    public class RatingReviewRepo : GenericRepo<RatingReview>, IRatingReviewRepo
    {
        public RatingReviewRepo(EVRentalDBContext context) : base(context)
        {
        }

        public async Task<IEnumerable<RatingReview>> GetAllWithDetailsAsync()
        {
            return await _dbContext.RatingReviews
                .Include(r => r.RentalRecord)
                    .ThenInclude(rr => rr.Renter)
                .Include(r => r.RentalRecord)
                    .ThenInclude(rr => rr.Vehicle)
                .OrderByDescending(r => r.CreateDate)
                .ToListAsync();
        }

        public async Task<RatingReview?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbContext.RatingReviews
                .Include(r => r.RentalRecord)
                    .ThenInclude(rr => rr.Renter)
                .Include(r => r.RentalRecord)
                    .ThenInclude(rr => rr.Vehicle)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<RatingReview>> GetByVehicleIdAsync(int vehicleId)
        {
            return await _dbContext.RatingReviews
                .Include(r => r.RentalRecord)
                    .ThenInclude(rr => rr.Renter)
                .Where(r => r.RentalRecord.VehicleId == vehicleId)
                .OrderByDescending(r => r.CreateDate)
                .ToListAsync();
        }

        public async Task<RatingReview?> GetByRentalIdAsync(int rentalId)
        {
            return await _dbContext.RatingReviews
                .Include(r => r.RentalRecord)
                    .ThenInclude(rr => rr.Renter)
                .Include(r => r.RentalRecord)
                    .ThenInclude(rr => rr.Vehicle)
                .FirstOrDefaultAsync(r => r.RentalId == rentalId);
        }

        public async Task<double> GetAverageRatingForVehicleAsync(int vehicleId)
        {
            var ratings = await _dbContext.RatingReviews
                .Where(r => r.RentalRecord.VehicleId == vehicleId)
                .Select(r => r.Rating)
                .ToListAsync();

            return ratings.Any() ? ratings.Average() : 0;
        }

        public async Task<Dictionary<int, int>> GetRatingDistributionAsync()
        {
            var distribution = await _dbContext.RatingReviews
                .GroupBy(r => r.Rating)
                .Select(g => new { Rating = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Rating, x => x.Count);

            // Đảm bảo có đầy đủ các rating từ 1-5
            for (int i = 1; i <= 5; i++)
            {
                if (!distribution.ContainsKey(i))
                {
                    distribution[i] = 0;
                }
            }

            return distribution;
        }
    }
}
