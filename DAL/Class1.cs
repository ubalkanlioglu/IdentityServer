using Api.Database.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Linq;
using JetBrains.Annotations;
using System.Threading.Tasks;

namespace DAL
{

    public class ApiContextFactory : IDesignTimeDbContextFactory<ApiContext>
    {
        private const string ConnectionString = "Server=./;Database=test1;Trusted_Connection=True;MultipleActiveResultSets=true";

        public ApiContext Create(DbContextFactoryOptions options)
        {
            var builder = new DbContextOptionsBuilder<ApiContext>();
            builder.UseSqlServer(
                ConnectionString);

            return new ApiContext(builder.Options);
        }

        public ApiContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ApiContext>();
            builder.UseSqlServer(
                ConnectionString);

            return new ApiContext(builder.Options);
        }
    }

    public class ApiContext : DbContext
    {
        public ApiContext(DbContextOptions options) : base(options) { }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.HasDefaultSchema(schema: DBGlobals.SchemaName);

            base.OnModelCreating(modelBuilder);


        }

        public override int SaveChanges()
        {
            AddAuitInfo();
            return base.SaveChanges();
        }

        //public async Task SaveChangesAsync()
        //{
        //    AddAuitInfo();
        //    return await base.SaveChangesAsync();
        //}

        private void AddAuitInfo()
        {
            var entries = ChangeTracker.Entries().Where(x => x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));
            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    ((BaseEntity)entry.Entity).Created = DateTime.UtcNow;
                }
            ((BaseEntity)entry.Entity).Modified = DateTime.UtcNow;
            }
        }
    }

    public static class DBGlobals
    {
        public const string SchemaName = "Portal";
    }
}
