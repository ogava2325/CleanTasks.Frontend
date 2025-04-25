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
        [Query] PaginationParameters paginationParameters,
        [Query] DateTimeOffset? startDate,
        [Query] DateTimeOffset? endDate,
        [Query] bool includeArchived,
        [Header("Authorization")] string authorization
    );

    [Get("/api/projects/archived")]
    Task<PaginatedList<ProjectDto>> GetArchivedByUserId(
        [Query] Guid userId,
        [Query] PaginationParameters paginationParameters,
        [Query] DateTimeOffset? startDate,
        [Query] DateTimeOffset? endDate,
        [Header("Authorization")] string authorization
    );
    
    [Get("/api/projects/{id}")]
    Task<ProjectDto> GetById(
        Guid id,
        [Header("Authorization")] string authorization
    );

    [Post("/api/projects")]
    Task<ProjectDto> CreateAsync(
        [Body] CreateProjectDto command,
        [Header("Authorization")] string authorization
    );

    [Delete("/api/projects/{id}")]
    Task DeleteAsync(
        Guid id,
        [Header("Authorization")] string authorization
    );

    [Put("/api/projects/{id}")]
    Task UpdateAsync(
        Guid id,
        [Body] UpdateProjectDto command,
        [Header("Authorization")] string authorization
    );

    [Post("/api/projects/{id}/users")]
    Task AddUserToProjectAsync(
        Guid id,
        [Body] AddUserToProjectsDto command,
        [Header("Authorization")] string authorization
    );
    
    [Put("/api/projects/{id}/archive")]
    Task ArchiveAsync(
        Guid id,
        [Header("Authorization")] string authorization
    );
    
    [Put("/api/projects/{id}/restore")]
    Task RestoreAsync(
        Guid id,
        [Header("Authorization")] string authorization
    );
}