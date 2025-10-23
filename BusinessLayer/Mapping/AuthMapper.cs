using BusinessLayer.DTOs;
using DataAccessLayer.Entities;

namespace BusinessLayer.Mapping
{
    public static class AuthMapper
    {
        /// <summary>
        /// Map từ Account sang AuthResponseDto
        /// </summary>
        public static AuthResponseDto ToAuthResponseDto(Account account, Renter? renter = null, Staff? staff = null)
        {
            var response = new AuthResponseDto
            {
                AccountId = account.Id,
                Email = account.Email,
                Phone = account.Phone,
                Role = account.Role,
                Status = account.Status
            };

            // Map thông tin Renter nếu có
            if (renter != null)
            {
                response.RenterId = renter.Id;
                response.RenterIdCard = renter.IdCard;
                response.RenterDriverLicense = renter.DriverLicense;
            }

            // Map thông tin Staff nếu có
            if (staff != null)
            {
                response.StaffId = staff.Id;
                response.StaffName = staff.FullName;
            }

            return response;
        }

        /// <summary>
        /// Tạo Account entity từ RegisterRequestDto
        /// </summary>
        public static Account ToAccountEntity(RegisterRequestDto request)
        {
            return new Account
            {
                Email = request.Email,
                Phone = request.Phone,
                Password = request.Password, // TODO: Hash password with BCrypt
                Role = DataAccessLayer.Enums.AccountRole.Renter,
                Status = DataAccessLayer.Enums.AccountStatus.Active,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
                IsDeleted = false
            };
        }

        /// <summary>
        /// Tạo Renter entity từ RegisterRequestDto
        /// </summary>
        public static Renter ToRenterEntity(RegisterRequestDto request, int accountId)
        {
            return new Renter
            {
                AccountId = accountId,
                IdCard = request.IdCard,
                DriverLicense = request.DriverLicense,
                Status = DataAccessLayer.Enums.RenterStatus.Pending,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
                IsDeleted = false
            };
        }
    }
}
