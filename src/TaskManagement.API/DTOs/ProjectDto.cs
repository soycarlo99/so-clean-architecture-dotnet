namespace TaskManagement.API.DTOs;

public class ProjectDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TaskItemsCount { get; set; }
}
