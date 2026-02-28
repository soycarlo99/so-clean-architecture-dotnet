namespace TaskManagement.API.DTOs;

/// <summary>
/// DTO for PATCH operations - all fields are optional.
/// Only include fields you want to update.
/// </summary>
public class UpdateTaskItemDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; }
    public decimal? EstimatedHours { get; set; }
    public int? AssignedToUserId { get; set; }
}
