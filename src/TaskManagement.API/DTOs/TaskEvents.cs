namespace TaskManagement.API.DTOs;

/// <summary>
/// Event sent when a new task is created.
/// </summary>
public class TaskCreatedEvent
{
    public string EventType => "TaskCreated";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public TaskItemDto Task { get; set; } = null!;
}

/// <summary>
/// Event sent when a task is updated.
/// </summary>
public class TaskUpdatedEvent
{
    public string EventType => "TaskUpdated";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public TaskItemDto Task { get; set; } = null!;
    public int UpdatedByUserId { get; set; }
    public string UpdatedByName { get; set; } = string.Empty;
}

/// <summary>
/// Event sent when a task is deleted.
/// </summary>
public class TaskDeletedEvent
{
    public string EventType => "TaskDeleted";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public int TaskId { get; set; }
    public int ProjectId { get; set; }
    public int DeletedByUserId { get; set; }
}
