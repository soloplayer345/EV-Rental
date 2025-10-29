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
                .AnyAsync(a => a.Phone == phone);
        }

        /// <summary>
        /// Lấy tất cả accounts theo role
        /// </summary>
        public async Task<IEnumerable<Account>> GetAccountsByRoleAsync(AccountRole role)
        {
            return await _dbSet
                .Where(x => x.Role == role)
                .OrderByDescending(x => x.CreateDate)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy tất cả accounts active
        /// </summary>
        public async Task<IEnumerable<Account>> GetActiveAccountsAsync()
        {
            return await _dbSet
                .Where(x => x.IsActive)
                .OrderByDescending(x => x.CreateDate)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy tất cả accounts inactive
        /// </summary>
        public async Task<IEnumerable<Account>> GetInactiveAccountsAsync()
        {
            return await _dbSet
                .Where(x => !x.IsActive)
                .OrderByDescending(x => x.CreateDate)
                .ToListAsync();
        }
    }
}
