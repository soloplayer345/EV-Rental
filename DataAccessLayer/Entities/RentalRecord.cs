using DataAccessLayer.Enums;

namespace DataAccessLayer.Entities
{
    public class RentalRecord : BaseEntity
    {
        public int RenterId { get; set; }
        public int VehicleId { get; set; }
        public int StationId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public decimal TotalCost { get; set; }
        public RentalRecordStatus Status { get; set; }
        public string ContractUrl { get; set; }

        // Navigation properties
        public virtual Renter Renter { get; set; }
        public virtual Vehicle Vehicle { get; set; }
        public virtual Station Station { get; set; }
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public virtual ICollection<VehicleCheck> VehicleChecks { get; set; } = new List<VehicleCheck>();
    }
}
