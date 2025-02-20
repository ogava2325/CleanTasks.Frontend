using Domain.Dtos.Project;
using Refit;

namespace Services.External;

public interface IProjectService
{
    [Get("/api/projects/{userId}")]
    Task<IEnumerable<ProjectDto>> GetByUserId(Guid userId);
    
    [Post("/api/projects")]
    Task<ProjectDto> CreateAsync([Body] CreateProjectDto command);
    
    [Delete("/api/projects/{id}")]
    Task DeleteAsync(Guid id);
}