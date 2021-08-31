using Ca.Core.Domain.Blog;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Ca.Data
{
    public class EfDbContext : DbContext
    {
        public EfDbContext(DbContextOptions<EfDbContext> options) : base(options)
        {
        }

        public DbSet<BlogPost> BlogPosts { get; set; }

        public DbSet<BlogComment> BlogComments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // Seed data
            modelBuilder.Entity<BlogPost>().HasData(new BlogPost
            {
                Title = "Welcome to my blog",
                Body = "Content here ...",
                CreatedOn = System.DateTime.UtcNow
            });
        }
    }
}