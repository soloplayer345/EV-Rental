using System.Linq.Expressions;

namespace DataAccessLayer.Interfaces
{
    public interface IGenericRepo<TModel> where TModel : BaseEntity
    {
        Task AddAsync(TModel model);
        void Update(TModel model);
        void Delete(TModel model);
        void SoftDelete(TModel model);
        Task<IEnumerable<TModel>> GetAllAsync();
        Task<TModel> GetByIdAsync(int id);
        IQueryable<TModel> GetAllQueryable(string includeProperties = "");
        Task<TModel> FindOneAsync(Expression<Func<TModel, bool>> predicate, string includeProperties = "");
    }
}
