using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using BusinessLayer.Mapping;
using DataAccessLayer.Entities;
using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class RentalRecordService : IRentalRecordService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RentalRecordService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<RentalRecordDto>> GetAllRentalRecordsAsync()
        {
            var rentalRepo = _unitOfWork.GetRepository<RentalRecord>();
            var records = rentalRepo.GetAllQueryable("Renter,Vehicle,PickupStation,ReturnStation,Payments");
            var recordList = await records.OrderByDescending(r => r.Id).ToListAsync();
            return RentalRecordMapper.ToDtoList(recordList);
        }

        public async Task<RentalRecordDto> GetRentalRecordByIdAsync(int id)
        {
            try
            {
                var rentalRepo = _unitOfWork.GetRepository<RentalRecord>();
                var record = await rentalRepo.FindOneAsync(r => r.Id == id, "Renter,Vehicle,PickupStation,ReturnStation,Payments,InspectionProblems");
                return RentalRecordMapper.ToDto(record);
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }

        public async Task UpdateRentalRecordAsync(RentalRecord rentalRecord)
        {
            var rentalRepo = _unitOfWork.GetRepository<RentalRecord>();
            rentalRepo.Update(rentalRecord);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<RentalRecordDto>> SearchRentalRecordsAsync(
            string searchQuery, 
            RentalRecordStatus? status, 
            DateTime? startDate, 
            DateTime? endDate)
        {
            var rentalRepo = _unitOfWork.GetRepository<RentalRecord>();
            var records = rentalRepo.GetAllQueryable("Renter,Vehicle,PickupStation,ReturnStation");
            
            var result = records.AsEnumerable();

            // Tìm kiếm theo tên người thuê hoặc biển số xe
            if (!string.IsNullOrEmpty(searchQuery))
            {
                result = result.Where(r =>
                    r.Renter.FullName.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                    r.Renter.Email.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                    r.Vehicle.PlateNumber.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                    r.Vehicle.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            // Lọc theo trạng thái
            if (status.HasValue)
            {
                result = result.Where(r => r.Status == status.Value).ToList();
            }

            // Lọc theo ngày bắt đầu
            if (startDate.HasValue)
            {
                result = result.Where(r => r.StartTime >= startDate.Value).ToList();
            }

            // Lọc theo ngày kết thúc
            if (endDate.HasValue)
            {
                result = result.Where(r => r.StartTime <= endDate.Value).ToList();
            }

            return RentalRecordMapper.ToDtoList(result.OrderByDescending(r => r.Id).ToList());
        }

        public decimal CalculateTotalPrice(RentalRecord record)
        {
            var total = record.BasePrice + record.DepositFee + record.ReservationFee + record.ExtraFees - record.Discount;
            return Math.Max(0, total);
        }
    }
}
