using TaskManagement.API.DTOs;

namespace TaskManagement.API.Services;

/// <summary>
/// Service for broadcasting real-time task notifications via SignalR.
/// </summary>
public interface ITaskNotificationService
{
    /// <summary>
    /// Broadcasts a task creation event to all connected clients and the project group.
    /// </summary>
    Task NotifyTaskCreatedAsync(TaskItemDto task);

    /// <summary>
    /// Broadcasts a task update event to relevant users (creator, assignee, project members).
    /// </summary>
    Task NotifyTaskUpdatedAsync(TaskItemDto task, int updatedByUserId, string updatedByName);

    /// <summary>
    /// Broadcasts a task deletion event to all connected clients and the project group.
    /// </summary>
    Task NotifyTaskDeletedAsync(int taskId, int projectId, int deletedByUserId);
}
