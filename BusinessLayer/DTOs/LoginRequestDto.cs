using System.ComponentModel.DataAnnotations;

namespace BusinessLayer.DTOs
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "Email hoặc số điện thoại là bắt buộc")]
        [Display(Name = "Email hoặc Số điện thoại")]
        public string EmailOrPhone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Ghi nhớ đăng nhập")]
        public bool RememberMe { get; set; } = false;
    }
}
