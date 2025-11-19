using Microsoft.EntityFrameworkCore.Storage;

namespace ThyroCareX.Infrastructure.InfrastructureBases
{
    public interface IGenericRepositoryAsync<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task SaveChangeAsync();
        IDbContextTransaction BeginTransaction();
        void Commit();
        void RollBack();
        IQueryable<T> GetTableNoTracking();
        IQueryable<T> GetTableAsTracking();
        Task<T> AddAsync(T entity);
        Task AddRangeAsync(ICollection<T> entities);
        Task UpdateAsync(T entity);
        Task UpdateRangeAsync(ICollection<T> entities);
        Task DeleteRangeAsync(ICollection<T> entities);
        Task DeleteAsync(T entity);
    }
}
