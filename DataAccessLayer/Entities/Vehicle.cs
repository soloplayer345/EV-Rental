using DataAccessLayer.Enums;

namespace DataAccessLayer.Entities
{
    public class Vehicle : BaseEntity
    {
        public string LicensePlate { get; set; }
        public float BatteryCapacity { get; set; }
        public int StationId { get; set; }
        public VehicleStatus Status { get; set; }
        public decimal PricePerHour { get; set; }

        // Navigation properties
        public virtual Station Station { get; set; }
        public virtual ICollection<RentalRecord> RentalRecords { get; set; } = new List<RentalRecord>();
    }
}
