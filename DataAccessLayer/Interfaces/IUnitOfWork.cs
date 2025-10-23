namespace DataAccessLayer.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Lấy generic repository cho entity bất kỳ
        /// </summary>
        IGenericRepo<TModel> GetRepository<TModel>() where TModel : BaseEntity;

        /// <summary>
        /// Lấy AccountRepo với các methods đặc thù
        /// </summary>
        IAccountRepo AccountRepo { get; }

        /// <summary>
        /// Lưu tất cả thay đổi (async)
        /// </summary>
        Task<int> SaveChangesAsync();

        /// <summary>
        /// Lưu tất cả thay đổi (sync)
        /// </summary>
        int SaveChanges();

        /// <summary>
        /// Bắt đầu transaction
        /// </summary>
        Task BeginTransactionAsync();

        /// <summary>
        /// Commit transaction
        /// </summary>
        Task CommitTransactionAsync();

        /// <summary>
        /// Rollback transaction
        /// </summary>
        Task RollbackTransactionAsync();
    }
}
