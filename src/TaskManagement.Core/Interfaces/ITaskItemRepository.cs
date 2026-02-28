using TaskManagement.Core.Entities;
using TaskManagement.Core.Enums;

namespace TaskManagement.Core.Interfaces;

public interface ITaskItemRepository
{
    Task<List<TaskItem>> GetAllAsync();
    Task<TaskItem?> GetByIdAsync(int id);
    Task<TaskItem> CreateAsync(TaskItem taskItem);
    Task UpdateAsync(TaskItem taskItem);
    Task DeleteAsync(int id);
    Task<(List<TaskItem> Items, int TotalCount)> GetFilteredAsync(
        TaskItemStatus? status,
        int? projectId,
        int? assignedToUserId,
        string? searchTerm,
        int page,
        int pageSize);
}
