using TaskManagement.Core.Enums;

namespace TaskManagement.Core.Entities;

public class TaskItem
{
    public int Id { get; private set; }

    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public TaskItemStatus Status { get; private set; }
    public decimal? EstimatedHours { get; private set; }

    public int CreatedByUserId { get; private set; }
    public User CreatedBy { get; private set; } = null!;

    public int? AssignedToUserId { get; private set; }
    public User? AssignedTo { get; private set; }

    public int ProjectId { get; private set; }
    public Project Project { get; private set; } = null!;

    // Timestamps
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    private TaskItem() { }

    // Public factory method
    public static TaskItem Create(string title, string? description, int projectId, int createdByUserId)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));

        if (title.Length > 200)
            throw new ArgumentException("Title cannot exceed 200 characters", nameof(title));

        return new TaskItem
        {
            Title = title,
            Description = description,
            ProjectId = projectId,
            CreatedByUserId = createdByUserId,
            Status = TaskItemStatus.Pending,
            CreatedAt = DateTime.UtcNow,
        };
    }

    public void UpdateTitle(string newTitle)
    {
        if (string.IsNullOrWhiteSpace(newTitle))
            throw new ArgumentException("Title cannot be empty");

        if (newTitle.Length > 200)
            throw new ArgumentException("Title cannot exceed 200 characters");

        Title = newTitle;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDescription(string? newDescription)
    {
        Description = newDescription;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignTo(int userId)
    {
        AssignedToUserId = userId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Unassign()
    {
        AssignedToUserId = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsInProgress()
    {
        if (Status != TaskItemStatus.Pending)
            throw new InvalidOperationException("Only pending tasks can be started");

        Status = TaskItemStatus.InProgress;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsInReview()
    {
        if (Status != TaskItemStatus.InProgress)
            throw new InvalidOperationException("Only in-progress tasks can be moved to review");

        Status = TaskItemStatus.InReview;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsComplete()
    {
        if (Status == TaskItemStatus.Done)
            throw new InvalidOperationException("Task is already completed");

        Status = TaskItemStatus.Done;
        CompletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetEstimate(decimal hours)
    {
        if (hours <= 0)
            throw new ArgumentException("Estimated hours must be positive");

        EstimatedHours = hours;
        UpdatedAt = DateTime.UtcNow;
    }
}
