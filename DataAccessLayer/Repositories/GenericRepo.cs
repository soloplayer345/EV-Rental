using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DataAccessLayer.Repositories
{
    public class GenericRepo<TModel> : IGenericRepo<TModel> where TModel : BaseEntity
    {
        protected readonly DbSet<TModel> _dbSet;
        protected readonly EVRentalDBContext _dbContext;

        public GenericRepo(EVRentalDBContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _dbSet = dbContext.Set<TModel>();
        }

        /// <summary>
        /// Thêm entity mới vào database
        /// </summary>
        public async Task AddAsync(TModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.CreateDate = DateTime.UtcNow;
            model.UpdateDate = DateTime.UtcNow;
            model.IsDeleted = false;

            await _dbSet.AddAsync(model);
        }

        /// <summary>
        /// Xóa vật lý entity khỏi database (Hard Delete)
        /// </summary>
        public async Task Delete(TModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            _dbSet.Remove(model);
        }

        /// <summary>
        /// Lấy tất cả records (không bao gồm records đã soft delete)
        /// </summary>
        public async Task<IEnumerable<TModel>> GetAllAsync()
        {
            return await _dbSet
                .Where(x => !x.IsDeleted)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy entity theo ID
        /// </summary>
        public async Task<TModel> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID must be greater than 0", nameof(id));

            TModel? model = await _dbSet.FindAsync(id);
            
            if (model == null || model.IsDeleted)
            {
                throw new KeyNotFoundException($"{typeof(TModel).Name} with ID {id} not found");
            }
            
            return model;
        }

        /// <summary>
        /// Xóa logic entity (Soft Delete)
        /// </summary>
        public void SoftDelete(TModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (model.IsDeleted)
                throw new InvalidOperationException($"{typeof(TModel).Name} is already deleted");

            model.IsDeleted = true;
            model.UpdateDate = DateTime.UtcNow;
            _dbSet.Update(model);
        }

        /// <summary>
        /// Cập nhật entity
        /// </summary>
        public async Task Update(TModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (model.IsDeleted)
                throw new InvalidOperationException($"Cannot update deleted {typeof(TModel).Name}");

            model.UpdateDate = DateTime.UtcNow;
            _dbSet.Update(model);
        }

        /// <summary>
        /// Lấy IQueryable để build query phức tạp, có thể include navigation properties
        /// </summary>
        public virtual IQueryable<TModel> GetAllQueryable(string includeProperties = "")
        {
            IQueryable<TModel> query = _dbSet.Where(x => !x.IsDeleted);

            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(
                    new char[] { ',' }, 
                    StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty.Trim());
                }
            }

            return query;
        }

        /// <summary>
        /// Tìm một entity dựa theo điều kiện, có thể include navigation properties
        /// </summary>
        public async Task<TModel> FindOneAsync(Expression<Func<TModel, bool>> predicate, string includeProperties = "")
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            IQueryable<TModel> query = _dbSet.Where(x => !x.IsDeleted);

            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(
                    new char[] { ',' }, 
                    StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty.Trim());
                }
            }

            var result = await query.FirstOrDefaultAsync(predicate);
            
            if (result == null)
            {
                throw new KeyNotFoundException($"{typeof(TModel).Name} not found with the specified criteria");
            }

            return result;
        }
    }
}
