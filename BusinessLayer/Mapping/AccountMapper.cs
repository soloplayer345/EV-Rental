using BusinessLayer.DTOs;
using DataAccessLayer.Entities;

namespace BusinessLayer.Mapping
{
    public static class AccountMapper
    {
        public static AccountDto ToDto(this Account account)
        {
            if (account == null) return null;

            return new AccountDto
            {
                Id = account.Id,
                FullName = account.FullName,
                Email = account.Email,
                Phone = account.Phone,
                Role = account.Role,
                IsActive = account.IsActive,
                CreateDate = account.CreateDate,
                UpdateDate = account.UpdateDate
            };
        }

        public static Account ToEntity(this AccountDto dto)
        {
            if (dto == null) return null;

            return new Account
            {
                Id = dto.Id,
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone,
                Role = dto.Role,
                IsActive = dto.IsActive,
                CreateDate = dto.CreateDate,
                UpdateDate = dto.UpdateDate
            };
        }

        public static List<AccountDto> ToDtoList(IEnumerable<Account> accounts)
        {
            return accounts?.Select(a => a.ToDto()).ToList() ?? new List<AccountDto>();
        }
    }
}
