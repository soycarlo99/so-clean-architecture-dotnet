namespace TaskManagement.API.DTOs;

public class TaskItemQueryParameters
{
    public string? Status { get; set; }
    public int? ProjectId { get; set; }
    public int? AssignedToUserId { get; set; }
    public string? SearchTerm { get; set; }

    private int _page = 1;
    public int Page
    {
        get => _page;
        set => _page = value < 1 ? 1 : value;
    }

    private int _pageSize = 10;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > 50 ? 50 : (value < 1 ? 10 : value);
    }
}
