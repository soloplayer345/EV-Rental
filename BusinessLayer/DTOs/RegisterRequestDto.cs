using System.ComponentModel.DataAnnotations;

namespace BusinessLayer.DTOs
{
    public class RegisterRequestDto
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [RegularExpression(@"^0[0-9]{9,10}$", ErrorMessage = "Số điện thoại phải bắt đầu bằng 0 và có 10-11 số")]
        [Display(Name = "Số điện thoại")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        [Display(Name = "Xác nhận mật khẩu")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số CCCD là bắt buộc")]
        [StringLength(12, MinimumLength = 9, ErrorMessage = "Số CCCD phải có 9-12 số")]
        [Display(Name = "Số CCCD")]
        public string IdCard { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số giấy phép lái xe là bắt buộc")]
        [StringLength(20, ErrorMessage = "Số giấy phép lái xe không được vượt quá 20 ký tự")]
        [Display(Name = "Số giấy phép lái xe")]
        public string DriverLicense { get; set; } = string.Empty;
    }
}
