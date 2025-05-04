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
    Task<ResultDto<string>> AddUserToProjectAsync(
        Guid id,
        [Body] AddUserToProjectCommandDto command,
        [Header("Authorization")] string authorization
    );
    
    [Delete("/api/projects/{id}/users")]
    Task<ResultDto<string>> RemoveUserFromProjectAsync(
        Guid id,
        [Body] RemoveUserFromProjectDto command,
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
    
    [Get("/api/projects/{projectId}/users")]
    Task<PaginatedList<ProjectMemberModel>> GetProjectMembers(
        [Query] Guid projectId,
        [Query] PaginationParameters paginationParameters,
        [Header("Authorization")] string authorization
    );
    
    [Put("/api/projects/{id}/users/{userId}/role")]
    Task<ResultDto<string>> ChangeUserRoleAsync(
        [Query] Guid id,
        [Query] Guid userId,
        [Body] ChangeUserRoleDto command,
        [Header("Authorization")] string authorization
    );
}