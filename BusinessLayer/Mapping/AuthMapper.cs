using BusinessLayer.DTOs;
using DataAccessLayer.Entities;

namespace BusinessLayer.Mapping
{
    public static class AuthMapper
    {
        /// <summary>
        /// Map từ Account sang AuthResponseDto
        /// </summary>
        public static AuthResponseDto ToAuthResponseDto(Account account)
        {
            var response = new AuthResponseDto
            {
                AccountId = account.Id,
                FullName = account.FullName,
                Email = account.Email,
                Phone = account.Phone,
                Role = account.Role,
                IsActive = account.IsActive
            };

            return response;
        }

        /// <summary>
        /// Tạo Account entity từ RegisterRequestDto
        /// </summary>
        public static Account ToAccountEntity(RegisterRequestDto request)
        {
            return new Account
            {
                FullName = request.FullName,
                Email = request.Email,
                Phone = request.Phone,
                PasswordHash = request.Password,
                Role = DataAccessLayer.Enums.AccountRole.Renter,
                IsActive = true,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
                IsDeleted = false
            };
        }
    }
}
