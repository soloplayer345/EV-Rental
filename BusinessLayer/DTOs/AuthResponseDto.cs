using DataAccessLayer.Enums;

namespace BusinessLayer.DTOs
{
    public class AuthResponseDto
    {
        public int AccountId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public AccountRole Role { get; set; }
        public bool IsActive { get; set; }
    }
}
