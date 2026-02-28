namespace TaskManagement.API.DTOs;

/// <summary>
/// DTO for PATCH operations - all fields are optional.
/// Only include fields you want to update.
/// </summary>
public class UpdateUserDto
{
    public string? Email { get; set; }
    public string? FullName { get; set; }
}
