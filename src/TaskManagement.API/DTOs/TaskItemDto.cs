namespace TaskManagement.API.DTOs;

public class TaskItemDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal? EstimatedHours { get; set; }
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public int CreatedByUserId { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public int? AssignedToUserId { get; set; }
    public string? AssignedToName { get; set; }
    public DateTime CreatedAt { get; set; }
}
