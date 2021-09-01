using Ca.Core.Domain.Blog;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Ca.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<BlogPost> BlogPosts { get; set; }

        public DbSet<BlogComment> BlogComments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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