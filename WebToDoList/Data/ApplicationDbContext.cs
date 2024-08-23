using Microsoft.EntityFrameworkCore;
using WebToDoList.Models;

namespace WebToDoList.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Models.Task> Tasks { get; set; }
        public DbSet<SubTask> SubTasks { get; set; }
    }
}