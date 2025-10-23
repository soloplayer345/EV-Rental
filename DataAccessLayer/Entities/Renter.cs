using DataAccessLayer.Enums;

namespace DataAccessLayer.Entities
{
    public class Renter : BaseEntity
    {
        public int AccountId { get; set; }
        public string DriverLicense { get; set; }
        public string IdCard { get; set; } // CCCD
        public RenterStatus Status { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }

        // Navigation properties
        public virtual Account Account { get; set; }
        public virtual ICollection<RentalRecord> RentalRecords { get; set; } = new List<RentalRecord>();
    }
}
