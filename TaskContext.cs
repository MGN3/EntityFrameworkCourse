using EntityFrameworkCourse.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCourse {
	public class TaskContext : DbContext {

		public DbSet<Category> Categories { get; set; }
		//There is a need to specify the folder containing the class in order
		//to eliminate the ambiguity
		public DbSet<Models.Task> Tasks { get; set; }

		public TaskContext(DbContextOptions<TaskContext> options) : base(options) { }

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			modelBuilder.Entity<Category>(cat => {
				cat.ToTable("Category");
				cat.HasKey(p => p.CategoryId);
				cat.Property(p => p.Name).IsRequired().HasMaxLength(150);
				cat.Property(p => p.Description).HasMaxLength(500);
			});

			modelBuilder.Entity<Models.Task>(tasks => {
				tasks.ToTable("Task");
				tasks.HasKey(p => p.TaskId);
				tasks.HasOne(p => p.Category).WithMany(p => p.Tasks).HasForeignKey(p => p.CategoryId);
				tasks.Property(p => p.Title).IsRequired().HasMaxLength(200);
				tasks.Property(p => p.Description).HasMaxLength(500);
				tasks.Property(p => p.TaskPriority).IsRequired();
				tasks.Property(p => p.TaskCreated).IsRequired();
				tasks.Ignore(p => p.Resumen);
			});

		}
	}
}
