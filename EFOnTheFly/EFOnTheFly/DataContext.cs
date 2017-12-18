using System.Data.Entity;

namespace EFOnTheFly {
    internal class DataContext<T> : DbContext where T : class {
        public DbSet<T> Entities { get; set; }
    }
}
