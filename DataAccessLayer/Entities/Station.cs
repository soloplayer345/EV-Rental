namespace DataAccessLayer.Entities
{
    public class Station : BaseEntity
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string State { get; set; }

        // Navigation properties
        public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        public virtual ICollection<RentalRecord> PickupRentalRecords { get; set; } = new List<RentalRecord>();
        public virtual ICollection<RentalRecord> ReturnRentalRecords { get; set; } = new List<RentalRecord>();
    }
}
