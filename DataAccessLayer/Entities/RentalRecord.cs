using DataAccessLayer.Enums;

namespace DataAccessLayer.Entities
{
    public class RentalRecord : BaseEntity
    {
        public int RenterId { get; set; }
        public int VehicleId { get; set; }
        public int PickupStationId { get; set; }
        public int? ReturnStationId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? ExpectedEndTime { get; set; }
        public DateTime? ActualEndTime { get; set; }
        public RentalRecordStatus Status { get; set; } = RentalRecordStatus.Pending; // 'pending'|'active'|'completed'|'cancelled'
        public decimal BasePrice { get; set; }
        public decimal DepositFee { get; set; } // Phí cọc
        public decimal ReservationFee { get; set; } // Phí giữ chỗ
        public decimal ExtraFees { get; set; } = 0;
        public decimal Discount { get; set; } = 0;
        public decimal TotalPrice { get; set; }
        public string OtpCode { get; set; } // OTP code (6 characters)

        // Navigation properties
        public virtual Account Renter { get; set; }
        public virtual Vehicle Vehicle { get; set; }
        public virtual Station PickupStation { get; set; }
        public virtual Station ReturnStation { get; set; }
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public virtual ICollection<InspectionProblem> InspectionProblems { get; set; } = new List<InspectionProblem>();
        public virtual RatingReview RatingReview { get; set; }
    }
}
