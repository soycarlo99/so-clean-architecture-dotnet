namespace TaskManagement.Core.Entities;

public class Project
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public List<TaskItem> TaskItems { get; private set; } = new();

    // Private constructor to enforce factory method usage
    private Project() { }

    // Factory method for creating new projects
    public static Project Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Project name cannot be empty", nameof(name));

        if (name.Length > 100)
            throw new ArgumentException("Project name cannot exceed 100 characters", nameof(name));

        return new Project
        {
            Name = name
        };
    }

    // Method to update project name
    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Project name cannot be empty", nameof(newName));

        if (newName.Length > 100)
            throw new ArgumentException("Project name cannot exceed 100 characters", nameof(newName));

        Name = newName;
    }
}
