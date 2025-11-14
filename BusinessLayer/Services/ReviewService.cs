using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReviewService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResultDto<IEnumerable<RatingReviewDto>>> GetAllReviewsAsync()
        {
            try
            {
                var ratingReviewRepo = _unitOfWork.GetRepository<DataAccessLayer.Entities.RatingReview>() 
                    as DataAccessLayer.Interfaces.IRatingReviewRepo;

                if (ratingReviewRepo == null)
                {
                    return new ServiceResultDto<IEnumerable<RatingReviewDto>>
                    {
                        Success = false,
                        Message = "Repository not found"
                    };
                }

                var reviews = await ratingReviewRepo.GetAllWithDetailsAsync();
                
                var reviewDtos = reviews.Select(r => new RatingReviewDto
                {
                    Id = r.Id,
                    RentalId = r.RentalId,
                    Rating = r.Rating,
                    Comment = r.Comment ?? "",
                    CreatedAt = r.CreateDate,
                    RenterName = r.RentalRecord?.Renter?.FullName ?? "Unknown",
                    RenterEmail = r.RentalRecord?.Renter?.Email ?? "",
                    VehicleId = r.RentalRecord?.VehicleId ?? 0,
                    VehicleName = r.RentalRecord?.Vehicle?.Name ?? "Unknown",
                    VehiclePlateNumber = r.RentalRecord?.Vehicle?.PlateNumber ?? "",
                    VehicleImageUrl = r.RentalRecord?.Vehicle?.ImageUrl ?? "/images/default-vehicle.png"
                });

                return new ServiceResultDto<IEnumerable<RatingReviewDto>>
                {
                    Success = true,
                    Data = reviewDtos
                };
            }
            catch (Exception ex)
            {
                return new ServiceResultDto<IEnumerable<RatingReviewDto>>
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResultDto<ReviewStatisticsDto>> GetReviewStatisticsAsync()
        {
            try
            {
                var ratingReviewRepo = _unitOfWork.GetRepository<DataAccessLayer.Entities.RatingReview>() 
                    as DataAccessLayer.Interfaces.IRatingReviewRepo;

                if (ratingReviewRepo == null)
                {
                    return new ServiceResultDto<ReviewStatisticsDto>
                    {
                        Success = false,
                        Message = "Repository not found"
                    };
                }

                var distribution = await ratingReviewRepo.GetRatingDistributionAsync();
                var totalReviews = distribution.Values.Sum();
                var averageRating = totalReviews > 0 
                    ? distribution.Sum(kvp => kvp.Key * kvp.Value) / (double)totalReviews 
                    : 0;

                var statistics = new ReviewStatisticsDto
                {
                    TotalReviews = totalReviews,
                    AverageRating = Math.Round(averageRating, 2),
                    RatingDistribution = distribution,
                    FiveStarCount = distribution.GetValueOrDefault(5, 0),
                    FourStarCount = distribution.GetValueOrDefault(4, 0),
                    ThreeStarCount = distribution.GetValueOrDefault(3, 0),
                    TwoStarCount = distribution.GetValueOrDefault(2, 0),
                    OneStarCount = distribution.GetValueOrDefault(1, 0)
                };

                // Tính phần trăm
                if (totalReviews > 0)
                {
                    statistics.FiveStarPercentage = Math.Round((statistics.FiveStarCount / (double)totalReviews) * 100, 1);
                    statistics.FourStarPercentage = Math.Round((statistics.FourStarCount / (double)totalReviews) * 100, 1);
                    statistics.ThreeStarPercentage = Math.Round((statistics.ThreeStarCount / (double)totalReviews) * 100, 1);
                    statistics.TwoStarPercentage = Math.Round((statistics.TwoStarCount / (double)totalReviews) * 100, 1);
                    statistics.OneStarPercentage = Math.Round((statistics.OneStarCount / (double)totalReviews) * 100, 1);
                }

                return new ServiceResultDto<ReviewStatisticsDto>
                {
                    Success = true,
                    Data = statistics
                };
            }
            catch (Exception ex)
            {
                return new ServiceResultDto<ReviewStatisticsDto>
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResultDto<IEnumerable<RatingReviewDto>>> GetReviewsByVehicleAsync(int vehicleId)
        {
            try
            {
                var ratingReviewRepo = _unitOfWork.GetRepository<DataAccessLayer.Entities.RatingReview>() 
                    as DataAccessLayer.Interfaces.IRatingReviewRepo;

                if (ratingReviewRepo == null)
                {
                    return new ServiceResultDto<IEnumerable<RatingReviewDto>>
                    {
                        Success = false,
                        Message = "Repository not found"
                    };
                }

                var reviews = await ratingReviewRepo.GetByVehicleIdAsync(vehicleId);
                
                var reviewDtos = reviews.Select(r => new RatingReviewDto
                {
                    Id = r.Id,
                    RentalId = r.RentalId,
                    Rating = r.Rating,
                    Comment = r.Comment ?? "",
                    CreatedAt = r.CreateDate,
                    RenterName = r.RentalRecord?.Renter?.FullName ?? "Unknown",
                    RenterEmail = r.RentalRecord?.Renter?.Email ?? "",
                    VehicleId = r.RentalRecord?.VehicleId ?? 0,
                    VehicleName = r.RentalRecord?.Vehicle?.Name ?? "Unknown",
                    VehiclePlateNumber = r.RentalRecord?.Vehicle?.PlateNumber ?? "",
                    VehicleImageUrl = r.RentalRecord?.Vehicle?.ImageUrl ?? "/images/default-vehicle.png"
                });

                return new ServiceResultDto<IEnumerable<RatingReviewDto>>
                {
                    Success = true,
                    Data = reviewDtos
                };
            }
            catch (Exception ex)
            {
                return new ServiceResultDto<IEnumerable<RatingReviewDto>>
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }
    }
}
