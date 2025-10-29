using DataAccessLayer.Enums;

namespace DataAccessLayer.Entities
{
    public class Account : BaseEntity
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string PasswordHash { get; set; }
        public AccountRole Role { get; set; } // 'renter' | 'staff' | 'admin'
        public bool IsActive { get; set; } = false;

        // Navigation properties
        public virtual ICollection<RentalRecord> RentalRecords { get; set; } = new List<RentalRecord>();
    }
}
