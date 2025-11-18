using BusinessLayer.DTOs;
using BusinessLayer.Services;
using DataAccessLayer.Entities;
using DataAccessLayer.Entities;

namespace BusinessLayer.Interfaces
{
    public interface IAccountService
    {
        Task<IEnumerable<AccountDto>> GetAllAccountsAsync(bool excludeAdmins = false);
        Task<AccountDto?> GetAccountByIdAsync(int accountId);
        Task<AccountDto?> GetByIdAsync(int id);
        Task<AccountDto?> GetAccountByEmailAsync(string email);
        Task<AccountDto> AddAccountAsync(dynamic accountData);
        Task UpdateAccountAsync(Account account);
        Task ToggleAccountStatusAsync(int accountId, bool isActive);
        Task DeleteAccountAsync(int accountId);
        Task<UserStatsDto> GetUserStatisticsAsync();
        Task<IEnumerable<AccountDto>> SearchAccountsAsync(string? searchQuery, AccountRole? role, bool? isActive);
    }
}
