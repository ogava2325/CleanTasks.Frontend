using Domain.Dtos.Project;
using Domain.Dtos.Shared;
using Domain.Dtos.User;
using Refit;

namespace services.External;

public interface IProjectService
{
    [Get("/api/projects")]
    Task<PaginatedList<ProjectDto>> GetByUserId(
        [Query] Guid userId,
        [Query] int pageNumber,
        [Query] int pageSize,
        [Query] string? searchTerm,
        [Query] ProjectsSortBy sortBy,
        [Query] ProjectsSortOrder sortOrder,
        [Query] DateTimeOffset? startDate,
        [Query] DateTimeOffset? endDate
    );
    
    [Get("/api/projects/{id}")]
    Task<ProjectDto> GetById(Guid id);
    
    [Post("/api/projects")]
    Task<ProjectDto> CreateAsync([Body] CreateProjectDto command);

    [Delete("/api/projects/{id}")]
    Task DeleteAsync(Guid id);
    
    [Put("/api/projects/{id}")]
    Task UpdateAsync(
        Guid id, 
        [Body] UpdateProjectDto command,
        [Header("Authorization")] string authorization);
    
    [Post("/api/projects/{projectId}/users")]
    Task AddUserToProjectAsync(
        Guid projectId,
        [Body] AddUserToProjectsDto command,
        [Header("Authorization")] string authorization
    );
}