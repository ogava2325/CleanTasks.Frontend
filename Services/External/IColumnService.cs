using Domain.Dtos.Column;
using Refit;

namespace services.External;

public interface IColumnService
{
    [Get("/api/columns/{projectId}")]
    Task<IEnumerable<ColumnDto>> GetByProjectId(Guid projectId);
    
    [Post("/api/columns")]
    Task<ColumnDto> CreateAsync([Body] CreateColumnDto command);
    
    [Delete("/api/columns/{id}")]
    Task DeleteAsync(Guid id, [Header("Authorization")] string authorization);
}