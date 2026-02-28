using Microsoft.EntityFrameworkCore;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Enums;
using TaskManagement.Core.Interfaces;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Repositories;

public class TaskItemRepository : ITaskItemRepository
{
    private readonly ApplicationDbContext _context;

    public TaskItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TaskItem>> GetAllAsync()
    {
        return await _context.TaskItems
            .Include(t => t.Project)
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .ToListAsync();
    }

    public async Task<TaskItem?> GetByIdAsync(int id)
    {
        return await _context.TaskItems
            .Include(t => t.Project)
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<TaskItem> CreateAsync(TaskItem taskItem)
    {
        _context.TaskItems.Add(taskItem);
        await _context.SaveChangesAsync();
        return taskItem;
    }

    public async Task UpdateAsync(TaskItem taskItem)
    {
        _context.TaskItems.Update(taskItem);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var taskItem = await _context.TaskItems.FindAsync(id);
        if (taskItem != null)
        {
            _context.TaskItems.Remove(taskItem);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<(List<TaskItem> Items, int TotalCount)> GetFilteredAsync(
        TaskItemStatus? status,
        int? projectId,
        int? assignedToUserId,
        string? searchTerm,
        int page,
        int pageSize)
    {
        // Start with base query
        IQueryable<TaskItem> query = _context.TaskItems
            .Include(t => t.Project)
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo);

        // Apply filters conditionally
        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status.Value);
        }

        if (projectId.HasValue)
        {
            query = query.Where(t => t.ProjectId == projectId.Value);
        }

        if (assignedToUserId.HasValue)
        {
            query = query.Where(t => t.AssignedToUserId == assignedToUserId.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerSearchTerm = searchTerm.ToLower();
            query = query.Where(t =>
                t.Title.ToLower().Contains(lowerSearchTerm) ||
                (t.Description != null && t.Description.ToLower().Contains(lowerSearchTerm)));
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync();

        // Apply pagination
        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}
