using DataAccessLayer.Entities;
using DataAccessLayer.Enums;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories
{
    public class AccountRepo : GenericRepo<Account>, IAccountRepo
    {
        public AccountRepo(EVRentalDBContext dbContext) : base(dbContext)
        {
        }

        /// <summary>
        /// Tìm account theo email
        /// </summary>
        public async Task<Account?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or empty", nameof(email));

            return await _dbSet
                .Where(x => !x.IsDeleted)
                .FirstOrDefaultAsync(a => a.Email == email);
        }

        /// <summary>
        /// Tìm account theo phone
        /// </summary>
        public async Task<Account?> GetByPhoneAsync(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException("Phone cannot be null or empty", nameof(phone));

            return await _dbSet
                .Where(x => !x.IsDeleted)
                .FirstOrDefaultAsync(a => a.Phone == phone);
        }

        /// <summary>
        /// Kiểm tra email đã tồn tại chưa
        /// </summary>
        public async Task<bool> IsEmailExistsAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return await _dbSet
                .Where(x => !x.IsDeleted)
                .AnyAsync(a => a.Email == email);
        }

        /// <summary>
        /// Kiểm tra phone đã tồn tại chưa
        /// </summary>
        public async Task<bool> IsPhoneExistsAsync(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            return await _dbSet
                .Where(x => !x.IsDeleted)
                .AnyAsync(a => a.Phone == phone);
        }

        /// <summary>
        /// Lấy tất cả accounts theo role
        /// </summary>
        public async Task<IEnumerable<Account>> GetAccountsByRoleAsync(AccountRole role)
        {
            return await _dbSet
                .Where(x => !x.IsDeleted && x.Role == role)
                .OrderByDescending(x => x.CreateDate)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy tất cả accounts theo status
        /// </summary>
        public async Task<IEnumerable<Account>> GetAccountsByStatusAsync(AccountStatus status)
        {
            return await _dbSet
                .Where(x => !x.IsDeleted && x.Status == status)
                .OrderByDescending(x => x.CreateDate)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy account kèm theo Renter navigation property
        /// </summary>
        public async Task<Account?> GetAccountWithRenterAsync(int accountId)
        {
            if (accountId <= 0)
                throw new ArgumentException("Account ID must be greater than 0", nameof(accountId));

            return await _dbSet
                .Where(x => !x.IsDeleted)
                .Include(a => a.Renter)
                .FirstOrDefaultAsync(a => a.Id == accountId);
        }

        /// <summary>
        /// Lấy account kèm theo Staff navigation property
        /// </summary>
        public async Task<Account?> GetAccountWithStaffAsync(int accountId)
        {
            if (accountId <= 0)
                throw new ArgumentException("Account ID must be greater than 0", nameof(accountId));

            return await _dbSet
                .Where(x => !x.IsDeleted)
                .Include(a => a.Staff)
                .FirstOrDefaultAsync(a => a.Id == accountId);
        }
    }
}
