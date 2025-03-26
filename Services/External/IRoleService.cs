using Domain.Dtos.Roles;
using Refit;

namespace services.External;

public interface IRoleService
{
    [Get("/api/roles")]
    Task<IEnumerable<RoleDto>> GetAllAsync([Header("Authorization")] string authorization);

    [Get("/api/roles/{id}")]
    Task<RoleDto> GetByIdAsync(Guid id);

    [Post("/api/roles")]
    Task<RoleDto> CreateAsync([Body] CreateRoleDto command);

    [Put("/api/roles/{id}")]
    Task UpdateAsync(Guid id, [Body] UpdateRoleDto command);

    [Delete("/api/roles/{id}")]
    Task DeleteAsync(Guid id);
}