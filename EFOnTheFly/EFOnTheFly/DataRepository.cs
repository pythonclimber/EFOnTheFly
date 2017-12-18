using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace EFOnTheFly {
    public interface IDataRepository<T> {
        T Get(params object[] keys);

        List<T> Get(Func<T, bool> getFunction);

        List<T> GetByStoredProc(string storedProc, params object[] parameters);

        T Add(T entity);

        T Update(T entity);

        T Delete(T entity);
    }

    public class DataRepository<T> : IDataRepository<T> where T : class {
        public T Get(params object[] keys) {
            T entity;

            using (var context = new DataContext<T>()) {
                entity = context.Entities.Find(keys);
            }

            return entity;
        }

        public List<T> Get(Func<T, bool> getFunction) {
            List<T> entities;

            using (var context = new DataContext<T>()) {
                entities = context.Entities.AsNoTracking().Where(getFunction).ToList();
            }

            return entities;
        }

        public List<T> GetByStoredProc(string storedProc, params object[] parameters) {
            using (var context = new DataContext<T>()) {
                return context.Database.SqlQuery<T>(storedProc, parameters).ToList();
            }
        }

        public T Add(T entity) {
            using (var context = new DataContext<T>()) {
                context.Entities.Add(entity);
                context.SaveChanges();
            }

            return entity;
        }

        public T Update(T entity) {
            using (var context = new DataContext<T>()) {
                context.Entities.Attach(entity);
                context.Entry(entity).State = EntityState.Modified;
                context.SaveChanges();
            }

            return entity;
        }

        public T Delete(T entity) {
            using (var context = new DataContext<T>()) {
                context.Entities.Remove(entity);
                context.SaveChanges();
            }

            return entity;
        }
    }
}
