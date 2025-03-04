using Domain.Dtos.State;
using Refit;

namespace Services.External;

public interface IStateService
{
    [Get("/api/states/{cardId}")]
    Task<StateDto> GetByCardIdAsync(Guid cardId);
    
    [Post("/api/states")]
    Task<StateDto> CreateAsync([Body] CreateStateDto command);
    
    [Put("/api/states/{id}")]
    Task UpdateAsync(Guid id, [Body] UpdateStateDto command);
}
