using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using BusinessLayer.Mapping;
using DataAccessLayer.Entities;
using DataAccessLayer.Enums;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Services
{
    public class CheckInService : ICheckInService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CheckInService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CheckInResultDto> ProcessCheckInAsync(CheckInDto checkInDto)
        {
            try
            {
                var rentalRepo = _unitOfWork.GetRepository<RentalRecord>();
                var rental = await rentalRepo.GetAllQueryable("Vehicle,Renter,InspectionProblems")
                    .FirstOrDefaultAsync(r => r.Id == checkInDto.RentalRecordId);

                if (rental == null)
                {
                    return new CheckInResultDto
                    {
                        Success = false,
                        Message = "Không tìm thấy bản ghi thuê xe"
                    };
                }

                if (rental.Status != RentalRecordStatus.Active)
                {
                    return new CheckInResultDto
                    {
                        Success = false,
                        Message = $"Trạng thái thuê xe không hợp lệ: {rental.Status}"
                    };
                }

                if (rental.StartTime == null)
                {
                    return new CheckInResultDto
                    {
                        Success = false,
                        Message = "Thời gian bắt đầu thuê xe chưa được ghi nhận"
                    };
                }

                // 1. Tính toán chi phí sử dụng
                var actualEndTime = checkInDto.ActualReturnTime;
                var totalHours = (decimal)(actualEndTime - rental.StartTime.Value).TotalHours;
                var roundedHours = Math.Ceiling(totalHours);
                decimal usageCost = rental.BasePrice * roundedHours;

                // 2. Tính phí trả chậm (20% của giá cơ bản cho mỗi giờ trễ)
                decimal lateFee = 0;
                if (rental.ExpectedEndTime.HasValue && actualEndTime > rental.ExpectedEndTime.Value)
                {
                    var lateHours = (decimal)(actualEndTime - rental.ExpectedEndTime.Value).TotalHours;
                    lateFee = Math.Ceiling(lateHours) * rental.BasePrice * 0.2m;
                }

                // 3. Xử lý inspection problems và tính tổng tiền phạt
                decimal totalPenalty = 0;
                var inspectionProblems = new List<InspectionProblem>();
                
                if (checkInDto.InspectionProblems != null && checkInDto.InspectionProblems.Any())
                {
                    var problemRepo = _unitOfWork.GetRepository<InspectionProblem>();
                    
                    foreach (var problemDto in checkInDto.InspectionProblems)
                    {
                        var problem = new InspectionProblem
                        {
                            RentalId = rental.Id,
                            IncidentType = problemDto.IncidentType,
                            Description = problemDto.Description,
                            Evidence = problemDto.Evidence ?? string.Empty,
                            PenaltyAmount = problemDto.PenaltyAmount,
                            CreatedBy = checkInDto.CreatedBy,
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now
                        };
                        
                        await problemRepo.AddAsync(problem);
                        inspectionProblems.Add(problem);
                        totalPenalty += problemDto.PenaltyAmount;
                    }
                }

                // 4. Tính tổng chi phí
                decimal totalAmount = usageCost + lateFee + totalPenalty + checkInDto.ExtraFees - checkInDto.Discount;

                // 5. Xử lý tiền cọc
                decimal depositRefund = 0;
                decimal finalPayment = totalAmount;

                if (rental.DepositFee > 0)
                {
                    // Nếu tổng chi phí < tiền cọc → hoàn lại phần dư
                    if (totalAmount < rental.DepositFee)
                    {
                        depositRefund = rental.DepositFee - totalAmount;
                        finalPayment = 0;
                    }
                    else
                    {
                        // Nếu tổng chi phí >= tiền cọc → trừ vào cọc, thu thêm phần chênh lệch
                        finalPayment = totalAmount - rental.DepositFee;
                        depositRefund = 0;
                    }
                }

                // 6. Cập nhật rental record
                rental.ActualEndTime = actualEndTime;
                rental.ReturnStationId = checkInDto.ReturnStationId ?? rental.PickupStationId;
                rental.ExtraFees = checkInDto.ExtraFees;
                rental.Discount = checkInDto.Discount;
                rental.TotalPrice = totalAmount;
                rental.Status = RentalRecordStatus.Completed;
                rental.UpdateDate = DateTime.Now;

                // 7. Cập nhật trạng thái xe về Available
                if (rental.Vehicle != null)
                {
                    rental.Vehicle.Status = VehicleStatus.Available;
                    rental.Vehicle.UpdateDate = DateTime.Now;
                }

                // 8. Lưu thay đổi
                rentalRepo.Update(rental);
                await _unitOfWork.SaveChangesAsync();

                return new CheckInResultDto
                {
                    Success = true,
                    Message = "Check-in thành công",
                    RentalRecordId = rental.Id,
                    UsageCost = usageCost,
                    LateFee = lateFee,
                    TotalPenalty = totalPenalty,
                    ExtraFees = checkInDto.ExtraFees,
                    Discount = checkInDto.Discount,
                    TotalAmount = totalAmount,
                    DepositFee = rental.DepositFee,
                    DepositRefund = depositRefund,
                    FinalPayment = finalPayment,
                    InspectionProblems = inspectionProblems.Select(p => new InspectionProblemDto
                    {
                        Id = p.Id,
                        RentalId = p.RentalId,
                        IncidentType = p.IncidentType,
                        Description = p.Description,
                        Evidence = p.Evidence,
                        PenaltyAmount = p.PenaltyAmount,
                        CreatedBy = p.CreatedBy,
                        CreateDate = p.CreateDate
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                return new CheckInResultDto
                {
                    Success = false,
                    Message = $"Lỗi khi xử lý check-in: {ex.Message}"
                };
            }
        }

        public async Task<RentalRecordDto> GetBillingAsync(int rentalRecordId)
        {
            try
            {
                var rentalRepo = _unitOfWork.GetRepository<RentalRecord>();
                var rental = await rentalRepo.GetAllQueryable("Vehicle,Renter,PickupStation,ReturnStation,InspectionProblems")
                    .FirstOrDefaultAsync(r => r.Id == rentalRecordId);

                if (rental == null)
                {
                    throw new ArgumentException("Không tìm thấy bản ghi thuê xe");
                }

                return rental.ToDto();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thông tin billing: {ex.Message}");
            }
        }

        public async Task<List<RentalRecordDto>> GetActiveRentalsAsync()
        {
            try
            {
                var rentalRepo = _unitOfWork.GetRepository<RentalRecord>();
                var activeRentals = await rentalRepo.GetAllQueryable("Vehicle,Renter,PickupStation")
                    .Where(r => r.Status == RentalRecordStatus.Active)
                    .OrderByDescending(r => r.StartTime)
                    .ToListAsync();

                return activeRentals.Select(r => r.ToDto()).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách thuê xe: {ex.Message}");
            }
        }

        public async Task<bool> ConfirmPaymentAsync(int rentalRecordId, decimal amount)
        {
            try
            {
                var rentalRepo = _unitOfWork.GetRepository<RentalRecord>();
                var rental = await rentalRepo.GetByIdAsync(rentalRecordId);

                if (rental == null)
                {
                    return false;
                }

                var paymentRepo = _unitOfWork.GetRepository<Payment>();
                await paymentRepo.AddAsync(new Payment
                {
                    RentalId = rentalRecordId,
                    Amount = amount,
                    Status = "paid",
                    PaidAt = DateTime.Now,
                    TransactionRef = Guid.NewGuid().ToString(),
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now
                });

                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
