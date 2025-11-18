using BusinessLayer.DTOs;

namespace BusinessLayer.Interfaces
{
    public interface IAuthService
    {
        /// <summary>
        /// Đăng nhập bằng email hoặc số điện thoại
        /// </summary>
        Task<ServiceResultDto<AuthResponseDto>> LoginAsync(LoginRequestDto request);

        /// <summary>
        /// Đăng ký tài khoản mới (tự động tạo role Renter)
        /// </summary>
        Task<ServiceResultDto<AuthResponseDto>> RegisterAsync(RegisterRequestDto request);

        /// <summary>
        /// Kiểm tra email đã tồn tại chưa
        /// </summary>
        Task<bool> IsEmailExistsAsync(string email);

        /// <summary>
        /// Kiểm tra số điện thoại đã tồn tại chưa
        /// </summary>
        Task<bool> IsPhoneExistsAsync(string phone);
    }
}
