using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;

namespace EFOnTheFly {

    internal class DataContext<TEntity> : DbContext where TEntity : class {
        private readonly EntityTypeConfiguration<TEntity> _entityTypeConfiguration;
        public DbSet<TEntity> Entities { get; set; }

        public DataContext(string nameOrConnectionString) : this(nameOrConnectionString, null) { }

        public DataContext(string nameOrConnectionString, EntityTypeConfiguration<TEntity> entityTypeConfiguration) : base(nameOrConnectionString) {
            _entityTypeConfiguration = entityTypeConfiguration;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            if (_entityTypeConfiguration != null) {
                modelBuilder.Configurations.Add(_entityTypeConfiguration);
            }
        }
    }
}
