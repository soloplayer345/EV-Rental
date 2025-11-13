using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer;
using DataAccessLayer.Entities;
using DataAccessLayer.Enums;
using DataAccessLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class CheckInService : ICheckInService
    {
        private readonly IUnitOfWork _unitOfWord;

        public CheckInService(IUnitOfWork unitOfWork)
        {
            _unitOfWord = unitOfWork;
        }

        public async Task<bool> ConfirmPaymentAsync(int rentalRecordId, decimal amount)
        {
            var rentalRepo = _unitOfWord.GetRepository<RentalRecord>();
            var rental = await rentalRepo.GetByIdAsync(rentalRecordId);

            if (rental == null) return false;

            var paymentRepo = _unitOfWord.GetRepository<Payment>();
            await paymentRepo.AddAsync(new Payment
            {
                RentalId = rentalRecordId,
                Amount = amount,
                Status = "paid",
                PaidAt = DateTime.Now,
                TransactionRef = Guid.NewGuid().ToString()
            });

            await _unitOfWord.SaveChangesAsync();
            return true;
        }

        public async Task<RentalRecordDto> GetBillingAsync(int rentalRecordId)
        {
            var rentalRepo = _unitOfWord.GetRepository<RentalRecord>();
            var rental = await rentalRepo.GetByIdAsync(rentalRecordId);

            if (rental == null)
            {
                throw new ArgumentException("Không tìm thấy bản ghi thuê xe");
            }

            var actualEnd = rental.ActualEndTime ?? DateTime.Now;
            var totalHours = (decimal)(actualEnd - rental.StartTime!.Value).TotalHours;
            var roundedHours = Math.Ceiling(totalHours);

            decimal usageCost = rental.BasePrice * (decimal)roundedHours;

            decimal lateFee = 0;
            if (rental.ExpectedEndTime.HasValue && actualEnd > rental.ExpectedEndTime.Value)
            {
                var lateHours = (decimal)(actualEnd - rental.ExpectedEndTime.Value).TotalHours;
                lateFee = Math.Ceiling(lateHours) * rental.BasePrice * 0.2m;
            }

            decimal finalAmount = usageCost + lateFee + rental.ExtraFees + rental.ReservationFee - rental.Discount;

            return new RentalRecordDto
            {
                Id = rental.Id,
                VehicleId = rental.VehicleId,
                RenterId = rental.RenterId,
                StartTime = rental.StartTime,
                ExpectedEndTime = rental.ExpectedEndTime,
                ActualEndTime = rental.ActualEndTime,
                Status = rental.Status,
                BasePrice = rental.BasePrice,
                DepositFee = rental.DepositFee,
                ReservationFee = rental.ReservationFee,
                ExtraFees = rental.ExtraFees,
                Discount = rental.Discount,
                TotalPrice = rental.TotalPrice,
                UsageTotal = usageCost,
                LateFee = lateFee,
                FinalAmount = finalAmount
            };
        }

        public async Task<bool> ProcessCheckInAsync(CheckInDto checkInDto)
        {
            var rentalRepo = _unitOfWord.GetRepository<RentalRecord>();
            var rental = await rentalRepo.GetByIdAsync(checkInDto.RentalRecordId);

            if (rental == null)
            {
                throw new ArgumentException("Không tìm thấy bản ghi thuê xe");
            }

            // update rental record
            rental.ActualEndTime = checkInDto.ActualReturnTime;

            if(rental.StartTime == null)
            {
                throw new InvalidOperationException("Thời gian thuê bắt đầu chưa được nhập");
            }

            var actualEnd = rental.ActualEndTime.Value;
            var totalHours = (decimal)(actualEnd - rental.StartTime.Value).TotalHours;
            var roundedHours = Math.Ceiling(totalHours);

            decimal usageCost = rental.BasePrice * (decimal)roundedHours;

            decimal lateFee = 0;
            if(rental.ExpectedEndTime.HasValue && actualEnd > rental.ExpectedEndTime.Value)
            {
                var lateHours = (decimal)(actualEnd - rental.ExpectedEndTime.Value).TotalHours;
                lateFee = (decimal)Math.Ceiling(lateHours) * rental.BasePrice * 0.2m; // 20% late fee
            }

            decimal damageCost = checkInDto.HasDamage ? checkInDto.DamageCost : 0;

            decimal total = usageCost + lateFee + checkInDto.ExtraFees + damageCost + rental.ReservationFee - checkInDto.Discount;

            rental.TotalPrice = total;
            rental.ExtraFees = checkInDto.ExtraFees;
            rental.Discount = checkInDto.Discount;
            rental.Status = RentalRecordStatus.Completed;

            rentalRepo.Update(rental);
            await _unitOfWord.SaveChangesAsync();
            return true;
        }        
    }
}
