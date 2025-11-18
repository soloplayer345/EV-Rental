using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using BusinessLayer.Mapping;
using DataAccessLayer.Entities;
using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccountService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Get all accounts (exclude admins for user management)
        public async Task<IEnumerable<AccountDto>> GetAllAccountsAsync(bool excludeAdmins = false)
        {
            var accountRepo = _unitOfWork.AccountRepo;
            var accounts = await accountRepo.GetAllAsync();

            if (excludeAdmins)
            {
                accounts = accounts.Where(a => a.Role != AccountRole.Admin).ToList();
            }

            return AccountMapper.ToDtoList(accounts);
        }

        // Get account by ID
        public async Task<AccountDto?> GetAccountByIdAsync(int id)
        {
            var accountRepo = _unitOfWork.AccountRepo;
            var account = await accountRepo.GetByIdAsync(id);
            return AccountMapper.ToDto(account);
        }

        // Get account by ID using Account interface method
        public async Task<AccountDto?> GetByIdAsync(int id)
        {
            var accountRepo = _unitOfWork.AccountRepo;
            var account = await accountRepo.GetByIdAsync(id);
            return AccountMapper.ToDto(account);
        }

        // Get account by email
        public async Task<AccountDto?> GetAccountByEmailAsync(string email)
        {
            var accountRepo = _unitOfWork.AccountRepo;
            var accounts = await accountRepo.GetAllAsync();
            var account = accounts.FirstOrDefault(a => a.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            return AccountMapper.ToDto(account);
        }

        // Add new account - accepts dynamic to support anonymous objects from Pages
        public async Task<AccountDto> AddAccountAsync(dynamic accountData)
        {
            var accountRepo = _unitOfWork.AccountRepo;
            
            // Convert dynamic to Account entity
            var account = new Account
            {
                FullName = accountData.FullName,
                Email = accountData.Email,
                Phone = accountData.Phone,
                PasswordHash = accountData.PasswordHash,
                Role = accountData.Role,
                IsActive = accountData.IsActive
            };
            
            // Check if email already exists
            if (await accountRepo.IsEmailExistsAsync(account.Email))
            {
                throw new InvalidOperationException("Email đã tồn tại");
            }

            // Check if phone already exists
            if (await accountRepo.IsPhoneExistsAsync(account.Phone))
            {
                throw new InvalidOperationException("Số điện thoại đã tồn tại");
            }

            account.CreateDate = DateTime.Now;
            account.UpdateDate = DateTime.Now;

            await accountRepo.AddAsync(account);
            await _unitOfWork.SaveChangesAsync();

            return AccountMapper.ToDto(account);
        }

        // Update account
        public async Task UpdateAccountAsync(Account account)
        {
            var accountRepo = _unitOfWork.AccountRepo;
            
            // Get existing account
            var existingAccount = await accountRepo.GetByIdAsync(account.Id);
            if (existingAccount == null)
            {
                throw new KeyNotFoundException("Không tìm thấy tài khoản");
            }

            // Check if email is changed and already exists
            if (existingAccount.Email != account.Email && await accountRepo.IsEmailExistsAsync(account.Email))
            {
                throw new InvalidOperationException("Email đã tồn tại");
            }

            // Check if phone is changed and already exists
            if (existingAccount.Phone != account.Phone && await accountRepo.IsPhoneExistsAsync(account.Phone))
            {
                throw new InvalidOperationException("Số điện thoại đã tồn tại");
            }

            account.UpdateDate = DateTime.Now;
            await accountRepo.Update(account);
            await _unitOfWork.SaveChangesAsync();
        }

        // Toggle account status (active/inactive)
        public async Task ToggleAccountStatusAsync(int accountId, bool isActive)
        {
            var accountRepo = _unitOfWork.AccountRepo;
            var account = await accountRepo.GetByIdAsync(accountId);
            
            if (account == null)
            {
                throw new KeyNotFoundException("Không tìm thấy tài khoản");
            }

            account.IsActive = isActive;
            account.UpdateDate = DateTime.Now;

            await accountRepo.Update(account);
            await _unitOfWork.SaveChangesAsync();
        }

        // Delete account
        public async Task DeleteAccountAsync(int accountId)
        {
            var accountRepo = _unitOfWork.AccountRepo;
            var account = await accountRepo.GetByIdAsync(accountId);
            
            if (account == null)
            {
                throw new KeyNotFoundException("Không tìm thấy tài khoản");
            }

            await accountRepo.Delete(account);
            await _unitOfWork.SaveChangesAsync();
        }

        // Get user statistics
        public async Task<UserStatsDto> GetUserStatisticsAsync()
        {
            var accountRepo = _unitOfWork.AccountRepo;
            var allAccounts = (await accountRepo.GetAllAsync())
                .Where(a => a.Role != AccountRole.Admin)
                .ToList();

            var firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            return new UserStatsDto
            {
                TotalUsers = allAccounts.Count,
                ActiveUsers = allAccounts.Count(a => a.IsActive),
                InactiveUsers = allAccounts.Count(a => !a.IsActive),
                NewUsersThisMonth = allAccounts.Count(a => a.CreateDate >= firstDayOfMonth)
            };
        }

        // Search accounts
        public async Task<IEnumerable<AccountDto>> SearchAccountsAsync(
            string? searchTerm = null,
            AccountRole? roleFilter = null,
            bool? statusFilter = null)
        {
            var accountRepo = _unitOfWork.AccountRepo;
            var accounts = (await accountRepo.GetAllAsync())
                .Where(a => a.Role != AccountRole.Admin)
                .AsEnumerable();

            // Search by name, email, or phone
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                accounts = accounts.Where(a =>
                    a.FullName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    a.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    a.Phone.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                );
            }

            // Filter by role
            if (roleFilter.HasValue)
            {
                accounts = accounts.Where(a => a.Role == roleFilter.Value);
            }

            // Filter by status
            if (statusFilter.HasValue)
            {
                accounts = accounts.Where(a => a.IsActive == statusFilter.Value);
            }

            return AccountMapper.ToDtoList(accounts.OrderByDescending(a => a.CreateDate).ToList());
        }
    }

    // DTO
    public class UserStatsDto
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }
        public int NewUsersThisMonth { get; set; }
    }
}
