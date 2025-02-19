using Domain.Dtos.Project;
using Refit;

namespace Services.External;

public interface IProjectService
{
    [Get("/api/projects/{userId}")]
    Task<IEnumerable<ProjectDto>> GetProjectsByUserId(Guid userId);
    
    [Post("/api/projects")]
    Task<ProjectDto> CreateProject([Body] CreateProjectDto command);
}