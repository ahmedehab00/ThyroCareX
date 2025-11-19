using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ThyroCareX.Infrastructure.Context;

namespace ThyroCareX.Infrastructure.InfrastructureBases
{
    public class GenericRepositoryAsync<T> : IGenericRepositoryAsync<T> where T : class
    {
        #region Prop
        private readonly ApplicationDbContext _dbcontext;
        #endregion
        #region Constructor

        public GenericRepositoryAsync(ApplicationDbContext dbcontext)
        {
            _dbcontext = dbcontext;

        }
        #endregion
        #region Handle Functions
        public virtual async Task<T> AddAsync(T entity)
        {
            await _dbcontext.Set<T>().AddAsync(entity);
            await _dbcontext.SaveChangesAsync();
            return entity;
        }

        public virtual async Task AddRangeAsync(ICollection<T> entities)
        {
            await _dbcontext.Set<T>().AddRangeAsync(entities);
            await _dbcontext.SaveChangesAsync();

        }

        public IDbContextTransaction BeginTransaction()
        {
            return _dbcontext.Database.BeginTransaction();

        }

        public void Commit()
        {
            _dbcontext.Database.CommitTransaction();
        }

        public void RollBack()
        {
            _dbcontext.Database.RollbackTransaction();
        }
        public virtual async Task DeleteAsync(T entity)
        {
            _dbcontext.Set<T>().Remove(entity);
            await _dbcontext.SaveChangesAsync();
        }

        public virtual async Task DeleteRangeAsync(ICollection<T> entities)
        {
            foreach (var entity in entities)
            {
                _dbcontext.Entry(entity).State = EntityState.Deleted;
            }
            await _dbcontext.SaveChangesAsync();

        }
        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbcontext.Set<T>().FindAsync(id);
        }

        public IQueryable<T> GetTableAsTracking()
        {
            return _dbcontext.Set<T>().AsQueryable();
        }

        public IQueryable<T> GetTableNoTracking()
        {
            return _dbcontext.Set<T>().AsNoTracking().AsQueryable();
        }

        public async Task SaveChangeAsync()
        {
            await _dbcontext.SaveChangesAsync();
        }

        public virtual async Task UpdateAsync(T entity)
        {
            _dbcontext.Set<T>().Update(entity);
            await _dbcontext.SaveChangesAsync();
        }

        public virtual async Task UpdateRangeAsync(ICollection<T> entities)
        {
            _dbcontext.Set<T>().UpdateRange(entities);
            await _dbcontext.SaveChangesAsync();
        }
        #endregion
    }
}