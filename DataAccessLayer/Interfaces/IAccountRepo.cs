using DataAccessLayer.Entities;

namespace DataAccessLayer.Interfaces
{
    public interface IAccountRepo : IGenericRepo<Account>
    {
        /// <summary>
        /// Tìm account theo email
        /// </summary>
        Task<Account?> GetByEmailAsync(string email);

        /// <summary>
        /// Tìm account theo phone
        /// </summary>
        Task<Account?> GetByPhoneAsync(string phone);

        /// <summary>
        /// Kiểm tra email đã tồn tại chưa
        /// </summary>
        Task<bool> IsEmailExistsAsync(string email);

        /// <summary>
        /// Kiểm tra phone đã tồn tại chưa
        /// </summary>
        Task<bool> IsPhoneExistsAsync(string phone);

        /// <summary>
        /// Lấy tất cả accounts theo role
        /// </summary>
        Task<IEnumerable<Account>> GetAccountsByRoleAsync(Enums.AccountRole role);

        /// <summary>
        /// Lấy tất cả accounts active
        /// </summary>
        Task<IEnumerable<Account>> GetActiveAccountsAsync();

        /// <summary>
        /// Lấy tất cả accounts inactive
        /// </summary>
        Task<IEnumerable<Account>> GetInactiveAccountsAsync();
    }
}
