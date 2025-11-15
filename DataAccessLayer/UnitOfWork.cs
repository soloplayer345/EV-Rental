using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace DataAccessLayer
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EVRentalDBContext _context;
        private readonly Dictionary<Type, object> _repositories;
        private IDbContextTransaction? _transaction;

        // Lazy initialization cho AccountRepo
        private IAccountRepo? _accountRepo;
        private IRatingReviewRepo? _ratingReviewRepo;
        private IVehicleRepo? _vehicleRepo;
        private IRentalrecordRepo? _rentalRecordRepo;
        private IGenericRepo<Payment>? _paymentRepo;
        public IVehicleRepo VehicleRepo
        {
            get
            {
                if (_vehicleRepo == null)
                {
                    _vehicleRepo = new VehicleRepo(_context);
                }
                return _vehicleRepo;
            }
        }

        public IRentalrecordRepo RentalRecordRepo
        {
            get
            {
                if (_rentalRecordRepo == null)
                {
                    _rentalRecordRepo = new RentalRecordRepo(_context);
                }
                return _rentalRecordRepo;
            }
        }

        public IGenericRepo<Payment> PaymentRepo
        {
            get
            {
                if (_paymentRepo == null)
                {
                    _paymentRepo = new GenericRepo<Payment>(_context);
                }
                return _paymentRepo;
            }
        }

        public UnitOfWork(EVRentalDBContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _repositories = new Dictionary<Type, object>();
        }

        /// <summary>
        /// Property để truy cập AccountRepo với các methods đặc thù
        /// </summary>
        public IAccountRepo AccountRepo
        {
            get
            {
                if (_accountRepo == null)
                {
                    _accountRepo = new AccountRepo(_context);
                }
                return _accountRepo;
            }
        }

        /// <summary>
        /// Lấy generic repository cho entity bất kỳ
        /// </summary>
        public IGenericRepo<TModel> GetRepository<TModel>() where TModel : BaseEntity
        {
            var type = typeof(TModel);
            if (!_repositories.ContainsKey(type))
            {
                // Kiểm tra xem có repository đặc thù cho entity này không
                if (type == typeof(RatingReview))
                {
                    if (_ratingReviewRepo == null)
                    {
                        _ratingReviewRepo = new RatingReviewRepo(_context);
                    }
                    _repositories[type] = _ratingReviewRepo;
                }
                else
                {
                    _repositories[type] = new GenericRepo<TModel>(_context);
                }
            }
            return (IGenericRepo<TModel>)_repositories[type];
        }

        /// <summary>
        /// Lưu tất cả thay đổi vào database (async)
        /// </summary>
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Lưu tất cả thay đổi vào database (sync)
        /// </summary>
        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        /// <summary>
        /// Bắt đầu transaction cho nhiều thao tác
        /// </summary>
        public async Task BeginTransactionAsync()
        {
            if (_transaction != null)
                throw new InvalidOperationException("Transaction already started");

            _transaction = await _context.Database.BeginTransactionAsync();
        }

        /// <summary>
        /// Commit transaction - lưu tất cả thay đổi
        /// </summary>
        public async Task CommitTransactionAsync()
        {
            if (_transaction == null)
                throw new InvalidOperationException("No transaction to commit");

            try
            {
                await _context.SaveChangesAsync();
                await _transaction.CommitAsync();
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        /// <summary>
        /// Rollback transaction - hủy tất cả thay đổi
        /// </summary>
        public async Task RollbackTransactionAsync()
        {
            if (_transaction == null)
                throw new InvalidOperationException("No transaction to rollback");

            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        /// <summary>
        /// Dispose resources
        /// </summary>
        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
