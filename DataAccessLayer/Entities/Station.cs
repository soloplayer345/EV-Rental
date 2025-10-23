namespace DataAccessLayer.Entities
{
    public class Station : BaseEntity
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }

        // Navigation properties
        public virtual ICollection<Staff> Staff { get; set; } = new List<Staff>();
        public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        public virtual ICollection<RentalRecord> RentalRecords { get; set; } = new List<RentalRecord>();
        public virtual Report Report { get; set; }
    }
}
