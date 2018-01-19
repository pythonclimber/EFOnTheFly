using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Linq;

namespace EFOnTheFly {
    public interface IDataService<TEntity> where TEntity : class {
        TEntity Get(params object[] keys);

        List<TEntity> Get(Func<TEntity, bool> getFunction);

        List<TEntity> GetByStoredProc(string storedProc, params object[] parameters);

        TEntity Add(TEntity entity);

        TEntity Update(TEntity entity);

        TEntity Delete(TEntity entity);

        int ExecuteStoredProc(string storedProc, params object[] parameters);
    }

    public class DataService<TEntity> : IDataService<TEntity> where TEntity : class {
        protected readonly string NameOrConnectionString;

        public DataService(string nameOrConnectionString) {
            NameOrConnectionString = nameOrConnectionString;
        }

        public TEntity Get(params object[] keys) {
            TEntity entity;

            using (var context = GetContext()) {
                entity = context.Set<TEntity>().Find(keys);
            }

            return entity;
        }

        public List<TEntity> Get(Func<TEntity, bool> getFunction) {
            List<TEntity> entities;

            using (var context = GetContext()) {
                entities = context.Set<TEntity>().AsNoTracking().Where(getFunction).ToList();
            }

            return entities;
        }

        public List<TEntity> GetByStoredProc(string storedProc, params object[] parameters) {
            using (var context = GetContext()) {
                return context.Database.SqlQuery<TEntity>(storedProc, parameters).ToList();
            }
        }

        public TEntity Add(TEntity entity) {
            using (var context = GetContext()) {
                context.Set<TEntity>().Add(entity);
                context.SaveChanges();
            }

            return entity;
        }

        public TEntity Update(TEntity entity) {
            using (var context = GetContext()) {
                context.Set<TEntity>().Attach(entity);
                context.Entry(entity).State = EntityState.Modified;
                context.SaveChanges();
            }

            return entity;
        }

        public TEntity Delete(TEntity entity) {
            using (var context = GetContext()) {
                context.Set<TEntity>().Remove(entity);
                context.SaveChanges();
            }

            return entity;
        }

        public int ExecuteStoredProc(string storedProc, params object[] parameters) {
            using (var context = GetContext()) {
                return context.Database.ExecuteSqlCommand(storedProc, parameters);
            }
        }

        protected virtual DbContext GetContext() {
            return new DataContext<TEntity>(NameOrConnectionString);
        }
    }

    public class ConfigurableDataService<TEntity, TEntityConfig> : DataService<TEntity> where TEntity : class
        where TEntityConfig : EntityTypeConfiguration<TEntity>, new() {
        public ConfigurableDataService(string nameOrConnectionString) : base(nameOrConnectionString) {
        }
        protected override DbContext GetContext() {
            return new DataContext<TEntity>(NameOrConnectionString, new TEntityConfig());
        }
    }
}
