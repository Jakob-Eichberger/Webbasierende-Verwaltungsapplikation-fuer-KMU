using System;
using System.Data.Entity.Infrastructure;
//using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Infrastructure;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Service
{
    public class BaseService
    {
        protected readonly Database _db;

        public BaseService(Database db)
        {
            _db = db;
        }

        public IQueryable<TEntity> GetTable<TEntity>() where TEntity : class => _db.Set<TEntity>();

        protected async Task AddAsync<TEntity>(TEntity obj) where TEntity : class
        {
            try
            {
                await _db.Set<TEntity>().AddAsync(obj);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) { throw new ApplicationException($"Bei der Operation ist ein fehler aufgetreten."); }
            catch (DbUpdateException) { throw new ApplicationException($"Bei der Operation ist ein fehler aufgetreten."); }
            catch (Exception) { throw; }
        }

        protected void Add<TEntity>(TEntity obj) where TEntity : class
        {
            try
            {
                _db.Set<TEntity>().Add(obj);
                _db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException) { throw new ApplicationException($"Bei der Operation ist ein fehler aufgetreten."); }
            catch (DbUpdateException) { throw new ApplicationException($"Bei der Operation ist ein fehler aufgetreten."); }
            catch (Exception) { throw; }
        }

        protected async Task DeleteAsync<TEntity>(TEntity obj) where TEntity : class
        {
            try
            {
                _db.Set<TEntity>().Remove(obj);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException) { throw new ApplicationException("Bei der Operation ist ein fehler aufgetreten."); }
            catch (Exception) { throw; }
        }

        protected void Update<TEntity>(TEntity obj) where TEntity : class
        {
            try
            {
                _db.Set<TEntity>().Update(obj);
                _db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException) { throw new ApplicationException($"Bei der Operation ist ein fehler aufgetreten."); }
            catch (DbUpdateException) { throw new ApplicationException($"Bei der Operation ist ein fehler aufgetreten."); }
            catch (Exception) { throw; }
        }

        protected async Task UpdateAsync<TEntity>(TEntity obj) where TEntity : class
        {
            try
            {
                _db.Set<TEntity>().Update(obj);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) { throw new ApplicationException($"Bei der Operation ist ein fehler aufgetreten."); }
            catch (DbUpdateException) { throw new ApplicationException($"Bei der Operation ist ein fehler aufgetreten."); }
            catch (Exception) { throw; }
        }

    }
}
