using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Revo.Domain.Common;
using Revo.Domain.Entities;
using Revo.Infrastructure.Identity;
using System.Reflection;

namespace Revo.Infrastructure.Database
{
    public class ApplicationDbContext
        : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public DbSet<Admin> Admins => Set<Admin>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<PortfolioItem> PortfolioItems => Set<PortfolioItem>();
        public DbSet<PortfolioMedia> PortfolioMedia => Set<PortfolioMedia>();
        public DbSet<Service> Services => Set<Service>();
        public DbSet<ContactRequest> ContactRequests => Set<ContactRequest>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
        override protected void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            base.OnModelCreating(builder);

            ApplySoftDeleteQueryFilters(builder);
        }

        private void ApplySoftDeleteQueryFilters(ModelBuilder modelBuilder)
        {
            var entitesImplementsISoftDeletable = modelBuilder.Model.GetEntityTypes()
                .Where(e => typeof(ISoftDeletable).IsAssignableFrom(e.ClrType));

            foreach (var entityType in entitesImplementsISoftDeletable)
            {
                var method = typeof(ApplicationDbContext)
                     .GetMethod(nameof(ConfigureSoftDeleteFilter), BindingFlags.NonPublic | BindingFlags.Instance);

                var genericMethod = method!.MakeGenericMethod(entityType.ClrType);
                genericMethod.Invoke(this, new object[] { modelBuilder });
            }
        }

        private void ConfigureSoftDeleteFilter<TEntity>(ModelBuilder builder) where TEntity : class, ISoftDeletable
        {
            builder.Entity<TEntity>().HasQueryFilter(e => !e.IsDeleted);
        }
     
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is ISoftDeletable softDeletableEntity && entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    softDeletableEntity.IsDeleted = true;
                }

                if (entry.Entity is AuditableEntity<Guid> auditableEntity)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditableEntity.CreatedAt = DateTime.UtcNow;
                            break;
                        case EntityState.Modified:
                            auditableEntity.UpdatedAt = DateTime.UtcNow;
                            break;
                    }
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}