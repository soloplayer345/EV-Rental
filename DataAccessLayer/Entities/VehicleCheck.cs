using DataAccessLayer.Enums;

namespace DataAccessLayer.Entities
{
    public class VehicleCheck : BaseEntity
    {
        public int RentalId { get; set; }
        public int StaffId { get; set; }
        public VehicleCheckType CheckType { get; set; }
        public string ConditionNote { get; set; }
        public string PhotoUrl { get; set; }
        public DateTime Timestamp { get; set; }

        // Navigation properties
        public virtual RentalRecord RentalRecord { get; set; }
        public virtual Staff Staff { get; set; }
    }
}
