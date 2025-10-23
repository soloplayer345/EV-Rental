namespace DataAccessLayer.Entities
{
    public class Staff : BaseEntity
    {
        public int AccountId { get; set; }
        public string FullName { get; set; }
        public int StationId { get; set; }
        public string Position { get; set; }

        // Navigation properties
        public virtual Account Account { get; set; }
        public virtual Station Station { get; set; }
        public virtual ICollection<VehicleCheck> VehicleChecks { get; set; } = new List<VehicleCheck>();
    }
}
