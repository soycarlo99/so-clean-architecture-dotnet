using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.DTOs;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces;

namespace TaskManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IProjectRepository _projectRepository;

    public ProjectsController(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProjectDto>>> GetAll()
    {
        var projects = await _projectRepository.GetAllAsync();

        var projectDtos = projects
            .Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                TaskItemsCount = p.TaskItems.Count,
            })
            .ToList();

        return Ok(projectDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectDto>> GetById(int id)
    {
        var project = await _projectRepository.GetByIdAsync(id);

        if (project == null)
            return NotFound();

        var projectDto = new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            TaskItemsCount = project.TaskItems.Count,
        };

        return Ok(projectDto);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectDto>> Create([FromBody] CreateProjectDto dto)
    {
        var project = Project.Create(dto.Name);

        var created = await _projectRepository.CreateAsync(project);

        var responseDto = new ProjectDto
        {
            Id = created.Id,
            Name = created.Name,
            TaskItemsCount = 0,
        };

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, responseDto);
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult<ProjectDto>> Update(int id, [FromBody] UpdateProjectDto dto)
    {
        var project = await _projectRepository.GetByIdAsync(id);

        if (project == null)
            return NotFound();

        if (dto.Name != null)
            project.UpdateName(dto.Name);

        await _projectRepository.UpdateAsync(project);

        var updated = await _projectRepository.GetByIdAsync(id);

        var responseDto = new ProjectDto
        {
            Id = updated!.Id,
            Name = updated.Name,
            TaskItemsCount = updated.TaskItems.Count,
        };

        return Ok(responseDto);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var project = await _projectRepository.GetByIdAsync(id);

        if (project == null)
            return NotFound();

        await _projectRepository.DeleteAsync(id);

        return NoContent();
    }
}
