namespace TaskManagement.API.DTOs;

public class CreateTaskItemDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int ProjectId { get; set; }
    // CreatedByUserId removed - now taken from JWT token
    public decimal? EstimatedHours { get; set; }
}
