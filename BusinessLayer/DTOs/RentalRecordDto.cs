using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class RentalRecordDto
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public string? VehicleName { get; set; }
        public int RenterId { get; set; }
        public int PickupStationId { get; set; }
        public int? ReturnStationId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? ExpectedEndTime { get; set; }
        public DateTime? ActualEndTime { get; set; }
        public RentalRecordStatus Status { get; set; }
        public decimal BasePrice { get; set; }
        public decimal DepositFee { get; set; }
        public decimal ReservationFee { get; set; }
        public decimal ExtraFees { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal UsageTotal { get; set; }
        public decimal LateFee { get; set; }
        public decimal FinalAmount { get; set; }
        public string OtpCode { get; set; } = string.Empty;
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        public string? StationNane { get; set; }

        // Navigation properties
        public AccountDto? Renter { get; set; }
        public VehicleDto? Vehicle { get; set; }
        public StationDto? PickupStation { get; set; }
        public StationDto? ReturnStation { get; set; }
        public ICollection<PaymentDto> Payments { get; set; } = new List<PaymentDto>();
        public ICollection<InspectionProblemDto> InspectionProblems { get; set; } = new List<InspectionProblemDto>();
    }
}
