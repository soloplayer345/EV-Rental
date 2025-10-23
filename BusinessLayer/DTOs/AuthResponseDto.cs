using DataAccessLayer.Enums;

namespace BusinessLayer.DTOs
{
    public class AuthResponseDto
    {
        public int AccountId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public AccountRole Role { get; set; }
        public AccountStatus Status { get; set; }
        public int? RenterId { get; set; }
        public string? RenterIdCard { get; set; }
        public string? RenterDriverLicense { get; set; }
        public int? StaffId { get; set; }
        public string? StaffName { get; set; }
    }
}
