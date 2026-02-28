using Microsoft.AspNetCore.SignalR;
using TaskManagement.API.DTOs;
using TaskManagement.API.Hubs;

namespace TaskManagement.API.Services;

/// <summary>
/// Implementation of ITaskNotificationService using SignalR.
/// Uses IHubContext to send messages from outside the Hub.
/// </summary>
public class TaskNotificationService : ITaskNotificationService
{
    private readonly IHubContext<TaskHub> _hubContext;

    public TaskNotificationService(IHubContext<TaskHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyTaskCreatedAsync(TaskItemDto task)
    {
        var taskEvent = new TaskCreatedEvent
        {
            Timestamp = DateTime.UtcNow,
            Task = task
        };

        // Broadcast to all connected users
        await _hubContext.Clients.Group("all_users")
            .SendAsync("TaskCreated", taskEvent);

        // Also send to the specific project group
        await _hubContext.Clients.Group($"project_{task.ProjectId}")
            .SendAsync("TaskCreated", taskEvent);
    }

    public async Task NotifyTaskUpdatedAsync(TaskItemDto task, int updatedByUserId, string updatedByName)
    {
        var taskEvent = new TaskUpdatedEvent
        {
            Timestamp = DateTime.UtcNow,
            Task = task,
            UpdatedByUserId = updatedByUserId,
            UpdatedByName = updatedByName
        };

        // Send to the project group
        await _hubContext.Clients.Group($"project_{task.ProjectId}")
            .SendAsync("TaskUpdated", taskEvent);

        // Send to the creator (if different from updater)
        if (task.CreatedByUserId != updatedByUserId)
        {
            await _hubContext.Clients.Group($"user_{task.CreatedByUserId}")
                .SendAsync("TaskUpdated", taskEvent);
        }

        // Send to the assignee (if exists and different from updater)
        if (task.AssignedToUserId.HasValue && task.AssignedToUserId.Value != updatedByUserId)
        {
            await _hubContext.Clients.Group($"user_{task.AssignedToUserId}")
                .SendAsync("TaskUpdated", taskEvent);
        }
    }

    public async Task NotifyTaskDeletedAsync(int taskId, int projectId, int deletedByUserId)
    {
        var taskEvent = new TaskDeletedEvent
        {
            Timestamp = DateTime.UtcNow,
            TaskId = taskId,
            ProjectId = projectId,
            DeletedByUserId = deletedByUserId
        };

        // Broadcast to all connected users
        await _hubContext.Clients.Group("all_users")
            .SendAsync("TaskDeleted", taskEvent);

        // Also send to the specific project group
        await _hubContext.Clients.Group($"project_{projectId}")
            .SendAsync("TaskDeleted", taskEvent);
    }
}
