using Domain.Dtos.Card;
using Refit;

namespace services.External;

public interface ICardService
{
    [Get("/api/cards/{columnId}")]
    Task<IEnumerable<CardDto>> GetByColumnId(Guid columnId);
    
    [Post("/api/cards")]
    Task<Guid> CreateAsync([Body] CreateCardDto command);
    
    [Get("/api/cards")]
    Task<IEnumerable<CardDto>> GetAll();
    
    [Put("/api/cards/{id}")]
    Task UpdateAsync(Guid id, [Body] UpdateCardDto command);

}