using Microsoft.EntityFrameworkCore;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Enums;

namespace TaskManagement.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<TaskItem> TaskItems => Set<TaskItem>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<User> Users => Set<User>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // UseSeeding (synchronous version)
        optionsBuilder.UseSeeding(
            (context, _) =>
            {
                var appContext = (ApplicationDbContext)context;
                if (!appContext.Users.Any())
                {
                    SeedData(appContext);
                }
            }
        );

        // UseAsyncSeeding (asynchronous version)
        optionsBuilder.UseAsyncSeeding(
            async (context, _, cancellationToken) =>
            {
                var appContext = (ApplicationDbContext)context;
                if (!await appContext.Users.AnyAsync(cancellationToken))
                {
                    await SeedDataAsync(appContext, cancellationToken);
                }
            }
        );
    }

    private void SeedData(ApplicationDbContext context)
    {
        // Seed users
        var user1 = CreateUser(1, "john@example.com", "John Developer");
        var user2 = CreateUser(2, "jane@example.com", "Jane Manager");
        context.Users.AddRange(user1, user2);
        context.SaveChanges();

        // Seed projects
        var project1 = CreateProject(1, "TaskManagement System");
        var project2 = CreateProject(2, "E-Commerce Platform");
        context.Projects.AddRange(project1, project2);
        context.SaveChanges();

        // Seed tasks using factory method
        var task1 = TaskItem.Create(
            "Setup Clean Architecture",
            "Create the project structure with Core, Infrastructure, and API layers",
            1,
            1
        );
        task1.SetEstimate(8);
        task1.MarkAsInProgress();
        task1.MarkAsInReview();
        task1.MarkAsComplete();

        var task2 = TaskItem.Create(
            "Implement Repository Pattern",
            "Create ITaskItemRepository and TaskItemRepository",
            1,
            1
        );
        task2.SetEstimate(5);
        task2.MarkAsInProgress();
        task2.AssignTo(2);

        var task3 = TaskItem.Create(
            "Add Authentication",
            "Implement JWT authentication with ASP.NET Core Identity",
            1,
            2
        );
        task3.SetEstimate(12);

        context.TaskItems.AddRange(task1, task2, task3);
        context.SaveChanges();
    }

    private async Task SeedDataAsync(
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        // Seed users
        var user1 = CreateUser(1, "john@example.com", "John Developer");
        var user2 = CreateUser(2, "jane@example.com", "Jane Manager");
        context.Users.AddRange(user1, user2);
        await context.SaveChangesAsync(cancellationToken);

        // Seed projects
        var project1 = CreateProject(1, "TaskManagement System");
        var project2 = CreateProject(2, "E-Commerce Platform");
        context.Projects.AddRange(project1, project2);
        await context.SaveChangesAsync(cancellationToken);

        // Seed tasks using factory method
        var task1 = TaskItem.Create(
            "Setup Clean Architecture",
            "Create the project structure with Core, Infrastructure, and API layers",
            1,
            1
        );
        task1.SetEstimate(8);
        task1.MarkAsInProgress();
        task1.MarkAsInReview();
        task1.MarkAsComplete();

        var task2 = TaskItem.Create(
            "Implement Repository Pattern",
            "Create ITaskItemRepository and TaskItemRepository",
            1,
            1
        );
        task2.SetEstimate(5);
        task2.MarkAsInProgress();
        task2.AssignTo(2);

        var task3 = TaskItem.Create(
            "Add Authentication",
            "Implement JWT authentication with ASP.NET Core Identity",
            1,
            2
        );
        task3.SetEstimate(12);

        context.TaskItems.AddRange(task1, task2, task3);
        await context.SaveChangesAsync(cancellationToken);
    }

    // Helper methods - use reflection to bypass private setters for seeding
    private static User CreateUser(int id, string email, string fullName)
    {
        var user = (User)
            System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(User));

        typeof(User).GetProperty(nameof(User.Id))!.SetValue(user, id);
        typeof(User).GetProperty(nameof(User.Email))!.SetValue(user, email);
        typeof(User).GetProperty(nameof(User.FullName))!.SetValue(user, fullName);
        // Default password "password123" hashed with BCrypt
        typeof(User).GetProperty(nameof(User.PasswordHash))!.SetValue(user,
            BCrypt.Net.BCrypt.HashPassword("password123"));

        return user;
    }

    private static Project CreateProject(int id, string name)
    {
        var project = (Project)
            System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(Project));

        typeof(Project).GetProperty(nameof(Project.Id))!.SetValue(project, id);
        typeof(Project).GetProperty(nameof(Project.Name))!.SetValue(project, name);

        return project;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Title).IsRequired().HasMaxLength(200);
            entity.Property(t => t.Description).HasMaxLength(2000);
            entity.Property(t => t.Status).HasConversion<int>();

            entity
                .HasOne(t => t.Project)
                .WithMany(p => p.TaskItems)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasOne(t => t.CreatedBy)
                .WithMany()
                .HasForeignKey(t => t.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(t => t.AssignedTo)
                .WithMany()
                .HasForeignKey(t => t.AssignedToUserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(256);
            entity.Property(u => u.FullName).IsRequired().HasMaxLength(100);
            entity.Property(u => u.PasswordHash).IsRequired().HasMaxLength(500);
        });
    }
}
