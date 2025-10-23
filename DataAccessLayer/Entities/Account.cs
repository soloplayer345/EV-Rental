using DataAccessLayer.Enums;

namespace DataAccessLayer.Entities
{
    public class Account : BaseEntity
    {
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public AccountRole Role { get; set; }
        public AccountStatus Status { get; set; }

        // Navigation properties
        public virtual Renter Renter { get; set; }
        public virtual Staff Staff { get; set; }
    }
}
