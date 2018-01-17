using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Linq;

namespace EFOnTheFly {
    public interface IDataRepository<TEntity> where TEntity : class {
        TEntity Get(params object[] keys);

        List<TEntity> Get(Func<TEntity, bool> getFunction);

        List<TEntity> GetByStoredProc(string storedProc, params object[] parameters);

        TEntity Add(TEntity entity);

        TEntity Update(TEntity entity);

        TEntity Delete(TEntity entity);
    }

    public class DataRepository<TEntity> : IDataRepository<TEntity> where TEntity : class {
        public TEntity Get(params object[] keys) {
            TEntity entity;

            using (var context = new DataContext<TEntity>()) {
                entity = context.Entities.Find(keys);
            }

            return entity;
        }

        public List<TEntity> Get(Func<TEntity, bool> getFunction) {
            List<TEntity> entities;

            using (var context = new DataContext<TEntity>()) {
                entities = context.Entities.AsNoTracking().Where(getFunction).ToList();
            }

            return entities;
        }

        public List<TEntity> GetByStoredProc(string storedProc, params object[] parameters) {
            using (var context = new DataContext<TEntity>()) {
                return context.Database.SqlQuery<TEntity>(storedProc, parameters).ToList();
            }
        }

        public TEntity Add(TEntity entity) {
            using (var context = new DataContext<TEntity>()) {
                context.Entities.Add(entity);
                context.SaveChanges();
            }

            return entity;
        }

        public TEntity Update(TEntity entity) {
            using (var context = new DataContext<TEntity>()) {
                context.Entities.Attach(entity);
                context.Entry(entity).State = EntityState.Modified;
                context.SaveChanges();
            }

            return entity;
        }

        public TEntity Delete(TEntity entity) {
            using (var context = new DataContext<TEntity>()) {
                context.Entities.Remove(entity);
                context.SaveChanges();
            }

            return entity;
        }

        protected virtual DbContext GetContext() {
            return new DataContext<TEntity>();
        }
    }

    public class ConfigurableDataRepository<TEntity, TEntityConfig> : DataRepository<TEntity> where TEntity : class
        where TEntityConfig : EntityTypeConfiguration<TEntity> { }
}
