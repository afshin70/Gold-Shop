using Gold.Domain.Entities.LoggingEntities;
using Gold.Infrastracture.Configurations.EntitiesConfig;
using Gold.Infrastracture.Configurations.SystemActivityEntitiesConfig;
using Microsoft.EntityFrameworkCore;

namespace Gold.Infrastracture.EFCoreContext
{
    public class ApplicationLoggerContext : DbContext
    {
        public ApplicationLoggerContext(DbContextOptions<ApplicationLoggerContext> options) : base(options)
        {

        }

        public DbSet<SystemActivityLog> SystemActivityLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SystemActivityLogConfig).Assembly);
            modelBuilder.HasDefaultSchema("dbo");
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}
