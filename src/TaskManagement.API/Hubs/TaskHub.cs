using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace TaskManagement.API.Hubs;

/// <summary>
/// SignalR Hub for real-time task updates.
/// Clients connect here to receive live notifications about task changes.
/// </summary>
[Authorize]
public class TaskHub : Hub
{
    /// <summary>
    /// Called when a client connects to the hub.
    /// We add them to a group based on their user ID for targeted notifications.
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;

        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, "all_users");

        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Called when a client disconnects from the hub.
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier;

        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
        }

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "all_users");

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Allows a client to join a project-specific group for project-scoped updates.
    /// </summary>
    public async Task JoinProjectGroup(int projectId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"project_{projectId}");
    }

    /// <summary>
    /// Allows a client to leave a project-specific group.
    /// </summary>
    public async Task LeaveProjectGroup(int projectId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"project_{projectId}");
    }
}
