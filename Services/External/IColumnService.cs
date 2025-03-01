using Domain.Dtos.Column;
using Refit;

namespace Services.External;

public interface IColumnService
{
    [Get("/api/columns/{projectId}")]
    Task<IEnumerable<ColumnDto>> GetByProjectId(Guid projectId);
    
    [Post("/api/columns")]
    Task<ColumnDto> CreateAsync([Body] CreateColumnDto command);
}