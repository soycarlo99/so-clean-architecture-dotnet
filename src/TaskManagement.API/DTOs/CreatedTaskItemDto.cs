namespace TaskManagement.API.DTOs;

/// <summary>
/// Minimal DTO returned after creating a task.
/// For full details with navigation properties, use GET /api/taskitems/{id}
/// </summary>
public class CreatedTaskItemDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal? EstimatedHours { get; set; }
    public int ProjectId { get; set; }
    public int CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
}
