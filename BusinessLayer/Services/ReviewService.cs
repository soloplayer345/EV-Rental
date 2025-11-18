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

        public async Task<ServiceResultDto<RatingReviewDto>> GetReviewByRentalIdAsync(int rentalId)
        {
            try
            {
                var ratingReviewRepo = _unitOfWork.GetRepository<DataAccessLayer.Entities.RatingReview>() 
                    as DataAccessLayer.Interfaces.IRatingReviewRepo;

                if (ratingReviewRepo == null)
                {
                    return new ServiceResultDto<RatingReviewDto>
                    {
                        Success = false,
                        Message = "Repository not found"
                    };
                }

                var review = await ratingReviewRepo.GetByRentalIdAsync(rentalId);
                
                if (review == null)
                {
                    return new ServiceResultDto<RatingReviewDto>
                    {
                        Success = false,
                        Message = "Chưa có đánh giá cho chuyến đi này"
                    };
                }

                var reviewDto = new RatingReviewDto
                {
                    Id = review.Id,
                    RentalId = review.RentalId,
                    Rating = review.Rating,
                    Comment = review.Comment ?? "",
                    CreatedAt = review.CreateDate,
                    RenterName = review.RentalRecord?.Renter?.FullName ?? "Unknown",
                    RenterEmail = review.RentalRecord?.Renter?.Email ?? "",
                    VehicleId = review.RentalRecord?.VehicleId ?? 0,
                    VehicleName = review.RentalRecord?.Vehicle?.Name ?? "Unknown",
                    VehiclePlateNumber = review.RentalRecord?.Vehicle?.PlateNumber ?? "",
                    VehicleImageUrl = review.RentalRecord?.Vehicle?.ImageUrl ?? "/images/default-vehicle.png"
                };

                return new ServiceResultDto<RatingReviewDto>
                {
                    Success = true,
                    Data = reviewDto
                };
            }
            catch (Exception ex)
            {
                return new ServiceResultDto<RatingReviewDto>
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResultDto<object>> CreateReviewAsync(CreateRatingReviewDto request, int renterId)
        {
            try
            {
                var ratingReviewRepo = _unitOfWork.GetRepository<DataAccessLayer.Entities.RatingReview>() 
                    as DataAccessLayer.Interfaces.IRatingReviewRepo;
                var rentalRecordRepo = _unitOfWork.GetRepository<DataAccessLayer.Entities.RentalRecord>() 
                    as DataAccessLayer.Interfaces.IRentalrecordRepo;

                if (ratingReviewRepo == null || rentalRecordRepo == null)
                {
                    return new ServiceResultDto<object>
                    {
                        Success = false,
                        Message = "Repository not found"
                    };
                }

                // Kiểm tra rental record có tồn tại
                var rental = await rentalRecordRepo.GetByIdAsync(request.RentalId);
                if (rental == null)
                {
                    return new ServiceResultDto<object>
                    {
                        Success = false,
                        Message = "Không tìm thấy chuyến đi"
                    };
                }

                // Kiểm tra rental thuộc về renter
                if (rental.RenterId != renterId)
                {
                    return new ServiceResultDto<object>
                    {
                        Success = false,
                        Message = "Bạn không có quyền đánh giá chuyến đi này"
                    };
                }

                // Kiểm tra rental đã hoàn thành chưa
                if (rental.Status != DataAccessLayer.Entities.RentalRecordStatus.Completed)
                {
                    return new ServiceResultDto<object>
                    {
                        Success = false,
                        Message = "Chỉ có thể đánh giá sau khi hoàn thành chuyến đi"
                    };
                }

                // Kiểm tra phí phạt đã thanh toán chưa
                if (rental.ExtraFees > 0)
                {
                    return new ServiceResultDto<object>
                    {
                        Success = false,
                        Message = "Vui lòng thanh toán phí phạt trước khi đánh giá"
                    };
                }

                // Kiểm tra đã đánh giá chưa
                var existingReview = await ratingReviewRepo.GetByRentalIdAsync(request.RentalId);
                if (existingReview != null)
                {
                    return new ServiceResultDto<object>
                    {
                        Success = false,
                        Message = "Bạn đã đánh giá chuyến đi này rồi"
                    };
                }

                // Tạo review mới
                var review = new DataAccessLayer.Entities.RatingReview
                {
                    RentalId = request.RentalId,
                    Rating = request.Rating,
                    Comment = request.Comment,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now
                };

                await ratingReviewRepo.AddAsync(review);
                await _unitOfWork.SaveChangesAsync();

                return new ServiceResultDto<object>
                {
                    Success = true,
                    Message = "Cảm ơn bạn đã đánh giá! Đánh giá của bạn đã được ghi nhận."
                };
            }
            catch (Exception ex)
            {
                return new ServiceResultDto<object>
                {
                    Success = false,
                    Message = $"Lỗi: {ex.Message}"
                };
            }
        }
    }
}
