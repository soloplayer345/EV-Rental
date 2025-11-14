using BusinessLayer.DTOs;
using DataAccessLayer.Entities;
using System.Linq;
using System.Collections.Generic;

namespace BusinessLayer.Mapping
{
    public static class RentalRecordMapper
    {
        public static RentalRecordDto ToDto(this RentalRecord record)
        {
            if (record == null) return null;

            return new RentalRecordDto
            {
                Id = record.Id,
                VehicleId = record.VehicleId,
                VehicleName = record.Vehicle?.Name,
                RenterId = record.RenterId,
                PickupStationId = record.PickupStationId,
                ReturnStationId = record.ReturnStationId,
                StartTime = record.StartTime,
                ExpectedEndTime = record.ExpectedEndTime,
                ActualEndTime = record.ActualEndTime,
                Status = record.Status,
                BasePrice = record.BasePrice,
                DepositFee = record.DepositFee,
                ReservationFee = record.ReservationFee,
                ExtraFees = record.ExtraFees,
                Discount = record.Discount,
                TotalPrice = record.TotalPrice,
                UsageTotal = 0,
                LateFee = 0,
                FinalAmount = record.TotalPrice,
                OtpCode = record.OtpCode ?? string.Empty,
                CreateDate = record.CreateDate,
                UpdateDate = record.UpdateDate,
                StationNane = record.PickupStation?.Name,
                
                // Navigation properties
                Renter = record.Renter != null ? AccountMapper.ToDto(record.Renter) : null,
                Vehicle = record.Vehicle != null ? VehicleMapper.ToVehicleDto(record.Vehicle) : null,
                PickupStation = record.PickupStation != null ? StationMapper.ToDto(record.PickupStation) : null,
                ReturnStation = record.ReturnStation != null ? StationMapper.ToDto(record.ReturnStation) : null,
                InspectionProblems = record.InspectionProblems?.Select(p => p.ToDto()).ToList() ?? new List<InspectionProblemDto>()
            };
        }

        public static List<RentalRecordDto> ToDtoList(IEnumerable<RentalRecord> records)
        {
            return records?.Select(r => r.ToDto()).ToList() ?? new List<RentalRecordDto>();
        }

        public static RentalRecord ToEntity(this RentalRecordDto dto)
        {
            if (dto == null) return null;

            return new RentalRecord
            {
                Id = dto.Id,
                VehicleId = dto.VehicleId,
                RenterId = dto.RenterId,
                PickupStationId = dto.PickupStationId,
                ReturnStationId = dto.ReturnStationId,
                StartTime = dto.StartTime,
                ExpectedEndTime = dto.ExpectedEndTime,
                ActualEndTime = dto.ActualEndTime,
                Status = dto.Status,
                BasePrice = dto.BasePrice,
                DepositFee = dto.DepositFee,
                ReservationFee = dto.ReservationFee,
                ExtraFees = dto.ExtraFees,
                Discount = dto.Discount,
                TotalPrice = dto.TotalPrice,
                OtpCode = dto.OtpCode,
                CreateDate = dto.CreateDate,
                UpdateDate = dto.UpdateDate
            };
        }
    }
}
