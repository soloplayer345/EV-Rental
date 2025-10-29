using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using BusinessLayer.Mapping;
using DataAccessLayer.Entities;
using DataAccessLayer.Enums;
using DataAccessLayer.Interfaces;

namespace BusinessLayer.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        /// <summary>
        /// Đăng nhập bằng email hoặc số điện thoại
        /// </summary>
        public async Task<ServiceResultDto<AuthResponseDto>> LoginAsync(LoginRequestDto request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.EmailOrPhone))
                    return ServiceResultDto<AuthResponseDto>.FailureResult("Email hoặc số điện thoại không được để trống");

                if (string.IsNullOrWhiteSpace(request.Password))
                    return ServiceResultDto<AuthResponseDto>.FailureResult("Mật khẩu không được để trống");

                // Tìm account theo email hoặc phone
                Account? account = null;

                // Kiểm tra xem là email hay phone
                if (request.EmailOrPhone.Contains("@"))
                {
                    account = await _unitOfWork.AccountRepo.GetByEmailAsync(request.EmailOrPhone);
                }
                else
                {
                    account = await _unitOfWork.AccountRepo.GetByPhoneAsync(request.EmailOrPhone);
                }

                // Kiểm tra account có tồn tại không
                if (account == null)
                    return ServiceResultDto<AuthResponseDto>.FailureResult("Email/Số điện thoại hoặc mật khẩu không chính xác");

                // Kiểm tra password (đơn giản - trong thực tế nên dùng BCrypt)
                // TODO: Implement password hashing with BCrypt
                if (account.PasswordHash != request.Password)
                    return ServiceResultDto<AuthResponseDto>.FailureResult("Email/Số điện thoại hoặc mật khẩu không chính xác");

                // Kiểm tra trạng thái account
                if (!account.IsActive)
                    return ServiceResultDto<AuthResponseDto>.FailureResult("Tài khoản chưa được kích hoạt hoặc đã bị vô hiệu hóa");

                var response = AuthMapper.ToAuthResponseDto(account);
                return ServiceResultDto<AuthResponseDto>.SuccessResult(response, "Đăng nhập thành công");
            }
            catch (Exception ex)
            {
                return ServiceResultDto<AuthResponseDto>.FailureResult("Đã có lỗi xảy ra khi đăng nhập", ex.Message);
            }
        }

        /// <summary>
        /// Đăng ký tài khoản mới (tự động tạo role Renter)
        /// </summary>
        public async Task<ServiceResultDto<AuthResponseDto>> RegisterAsync(RegisterRequestDto request)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Validate
                if (string.IsNullOrWhiteSpace(request.Email))
                    return ServiceResultDto<AuthResponseDto>.FailureResult("Email không được để trống");

                if (string.IsNullOrWhiteSpace(request.Phone))
                    return ServiceResultDto<AuthResponseDto>.FailureResult("Số điện thoại không được để trống");

                if (string.IsNullOrWhiteSpace(request.Password))
                    return ServiceResultDto<AuthResponseDto>.FailureResult("Mật khẩu không được để trống");

                if (request.Password != request.ConfirmPassword)
                    return ServiceResultDto<AuthResponseDto>.FailureResult("Mật khẩu xác nhận không khớp");

                if (string.IsNullOrWhiteSpace(request.IdCard))
                    return ServiceResultDto<AuthResponseDto>.FailureResult("Số CCCD không được để trống");

                if (string.IsNullOrWhiteSpace(request.DriverLicense))
                    return ServiceResultDto<AuthResponseDto>.FailureResult("Số giấy phép lái xe không được để trống");

                // Kiểm tra email đã tồn tại
                if (await _unitOfWork.AccountRepo.IsEmailExistsAsync(request.Email))
                    return ServiceResultDto<AuthResponseDto>.FailureResult("Email đã được sử dụng");

                // Kiểm tra phone đã tồn tại
                if (await _unitOfWork.AccountRepo.IsPhoneExistsAsync(request.Phone))
                    return ServiceResultDto<AuthResponseDto>.FailureResult("Số điện thoại đã được sử dụng");

                // Tạo Account từ DTO
                var account = AuthMapper.ToAccountEntity(request);
                await _unitOfWork.AccountRepo.AddAsync(account);
                await _unitOfWork.SaveChangesAsync(); // Save để lấy AccountId

                // Commit transaction
                await _unitOfWork.CommitTransactionAsync();

                // Map sang response DTO
                var response = AuthMapper.ToAuthResponseDto(account);
                return ServiceResultDto<AuthResponseDto>.SuccessResult(response, "Đăng ký thành công");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ServiceResultDto<AuthResponseDto>.FailureResult("Đã có lỗi xảy ra khi đăng ký", ex.Message);
            }
        }

        /// <summary>
        /// Kiểm tra email đã tồn tại chưa
        /// </summary>
        public async Task<bool> IsEmailExistsAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return await _unitOfWork.AccountRepo.IsEmailExistsAsync(email);
        }

        /// <summary>
        /// Kiểm tra số điện thoại đã tồn tại chưa
        /// </summary>
        public async Task<bool> IsPhoneExistsAsync(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            return await _unitOfWork.AccountRepo.IsPhoneExistsAsync(phone);
        }
    }
}
