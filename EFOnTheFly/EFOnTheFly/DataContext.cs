using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;

namespace EFOnTheFly {
    internal class DataContext<TEntity> : DbContext where TEntity : class {
        private readonly EntityTypeConfiguration<TEntity> _entityTypeConfiguration;
        public DbSet<TEntity> Entities { get; set; }

        public DataContext() { }

        public DataContext(EntityTypeConfiguration<TEntity> entityTypeConfiguration) {
            _entityTypeConfiguration = entityTypeConfiguration;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.Add(_entityTypeConfiguration);
        }
    }
}
