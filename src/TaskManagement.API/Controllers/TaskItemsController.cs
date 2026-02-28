using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.DTOs;
using TaskManagement.API.Extensions;
using TaskManagement.API.Services;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Enums;
using TaskManagement.Core.Interfaces;

namespace TaskManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TaskItemsController : ControllerBase
{
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly ITaskNotificationService _notificationService;

    public TaskItemsController(
        ITaskItemRepository taskItemRepository,
        ITaskNotificationService notificationService
    )
    {
        _taskItemRepository = taskItemRepository;
        _notificationService = notificationService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<TaskItemDto>>> GetAll(
        [FromQuery] TaskItemQueryParameters queryParams
    )
    {
        TaskItemStatus? status = null;
        if (!string.IsNullOrWhiteSpace(queryParams.Status))
        {
            if (
                Enum.TryParse<TaskItemStatus>(
                    queryParams.Status,
                    ignoreCase: true,
                    out var parsedStatus
                )
            )
            {
                status = parsedStatus;
            }
            else
            {
                return BadRequest(
                    $"Invalid status value. Valid values are: {string.Join(", ", Enum.GetNames<TaskItemStatus>())}"
                );
            }
        }

        // Get filtered and paginated results
        var (tasks, totalCount) = await _taskItemRepository.GetFilteredAsync(
            status,
            queryParams.ProjectId,
            queryParams.AssignedToUserId,
            queryParams.SearchTerm,
            queryParams.Page,
            queryParams.PageSize
        );

        // Map to DTOs
        var taskDtos = tasks
            .Select(t => new TaskItemDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status.ToString(),
                EstimatedHours = t.EstimatedHours,
                ProjectId = t.ProjectId,
                ProjectName = t.Project.Name,
                CreatedByUserId = t.CreatedByUserId,
                CreatedByName = t.CreatedBy.FullName,
                AssignedToUserId = t.AssignedToUserId,
                AssignedToName = t.AssignedTo?.FullName,
                CreatedAt = t.CreatedAt,
            })
            .ToList();

        // Create paged result
        var pagedResult = new PagedResult<TaskItemDto>(
            taskDtos,
            queryParams.Page,
            queryParams.PageSize,
            totalCount
        );

        return Ok(pagedResult);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskItemDto>> GetById(int id)
    {
        var task = await _taskItemRepository.GetByIdAsync(id);

        if (task == null)
            return NotFound();

        var taskDto = new TaskItemDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status.ToString(),
            EstimatedHours = task.EstimatedHours,
            ProjectId = task.ProjectId,
            ProjectName = task.Project.Name,
            CreatedByUserId = task.CreatedByUserId,
            CreatedByName = task.CreatedBy.FullName,
            AssignedToUserId = task.AssignedToUserId,
            AssignedToName = task.AssignedTo?.FullName,
            CreatedAt = task.CreatedAt,
        };

        return Ok(taskDto);
    }

    [HttpPost]
    public async Task<ActionResult<CreatedTaskItemDto>> Create([FromBody] CreateTaskItemDto dto)
    {
        // Use the authenticated user's ID as the creator
        var currentUserId = User.GetUserId();

        var taskItem = TaskItem.Create(dto.Title, dto.Description, dto.ProjectId, currentUserId);

        if (dto.EstimatedHours.HasValue)
            taskItem.SetEstimate(dto.EstimatedHours.Value);

        var created = await _taskItemRepository.CreateAsync(taskItem);

        var taskWithRelations = await _taskItemRepository.GetByIdAsync(created.Id);

        var fullDto = new TaskItemDto
        {
            Id = taskWithRelations!.Id,
            Title = taskWithRelations.Title,
            Description = taskWithRelations.Description,
            Status = taskWithRelations.Status.ToString(),
            EstimatedHours = taskWithRelations.EstimatedHours,
            ProjectId = taskWithRelations.ProjectId,
            ProjectName = taskWithRelations.Project.Name,
            CreatedByUserId = taskWithRelations.CreatedByUserId,
            CreatedByName = taskWithRelations.CreatedBy.FullName,
            AssignedToUserId = taskWithRelations.AssignedToUserId,
            AssignedToName = taskWithRelations.AssignedTo?.FullName,
            CreatedAt = taskWithRelations.CreatedAt,
        };

        // Broadcast real-time notification
        await _notificationService.NotifyTaskCreatedAsync(fullDto);

        // Map to minimal response DTO (no navigation properties needed)
        var responseDto = new CreatedTaskItemDto
        {
            Id = created.Id,
            Title = created.Title,
            Description = created.Description,
            Status = created.Status.ToString(),
            EstimatedHours = created.EstimatedHours,
            ProjectId = created.ProjectId,
            CreatedByUserId = created.CreatedByUserId,
            CreatedAt = created.CreatedAt,
        };

        // Return 201 Created with the new resource
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, responseDto);
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult<TaskItemDto>> Update(int id, [FromBody] UpdateTaskItemDto dto)
    {
        var taskItem = await _taskItemRepository.GetByIdAsync(id);

        if (taskItem == null)
            return NotFound();

        var currentUserId = User.GetUserId();
        if (taskItem.CreatedByUserId != currentUserId && taskItem.AssignedToUserId != currentUserId)
        {
            return Forbid();
        }

        if (dto.Title != null)
            taskItem.UpdateTitle(dto.Title);

        if (dto.Description != null)
            taskItem.UpdateDescription(dto.Description);

        if (dto.EstimatedHours.HasValue)
            taskItem.SetEstimate(dto.EstimatedHours.Value);

        if (dto.AssignedToUserId.HasValue)
            taskItem.AssignTo(dto.AssignedToUserId.Value);

        // Handle status transitions using domain methods
        if (dto.Status != null)
        {
            if (Enum.TryParse<TaskItemStatus>(dto.Status, out var targetStatus))
            {
                switch (targetStatus)
                {
                    case TaskItemStatus.InProgress:
                        taskItem.MarkAsInProgress();
                        break;
                    case TaskItemStatus.InReview:
                        taskItem.MarkAsInReview();
                        break;
                    case TaskItemStatus.Done:
                        taskItem.MarkAsComplete();
                        break;
                    case TaskItemStatus.Pending:
                        break;
                }
            }
        }

        await _taskItemRepository.UpdateAsync(taskItem);

        var updated = await _taskItemRepository.GetByIdAsync(id);

        var responseDto = new TaskItemDto
        {
            Id = updated!.Id,
            Title = updated.Title,
            Description = updated.Description,
            Status = updated.Status.ToString(),
            EstimatedHours = updated.EstimatedHours,
            ProjectId = updated.ProjectId,
            ProjectName = updated.Project.Name,
            CreatedByUserId = updated.CreatedByUserId,
            CreatedByName = updated.CreatedBy.FullName,
            AssignedToUserId = updated.AssignedToUserId,
            AssignedToName = updated.AssignedTo?.FullName,
            CreatedAt = updated.CreatedAt,
        };

        // Broadcast real-time notification
        var updaterName =
            User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? "Unknown";
        await _notificationService.NotifyTaskUpdatedAsync(responseDto, currentUserId, updaterName);

        return Ok(responseDto);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var taskItem = await _taskItemRepository.GetByIdAsync(id);

        if (taskItem == null)
            return NotFound();

        // Only creator can delete
        var currentUserId = User.GetUserId();
        if (taskItem.CreatedByUserId != currentUserId)
        {
            return Forbid();
        }

        var projectId = taskItem.ProjectId;

        await _taskItemRepository.DeleteAsync(id);

        // Broadcast real-time notification
        await _notificationService.NotifyTaskDeletedAsync(id, projectId, currentUserId);

        return NoContent();
    }
}
