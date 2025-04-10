using Domain.Dtos.Card;
using Refit;

namespace services.External;

public interface ICardService
{
    [Get("/api/cards/{columnId}")]
    Task<IEnumerable<CardDto>> GetByColumnId(
        Guid columnId,
        [Header("Authorization")] string authorization
    );

    [Post("/api/cards")]
    Task<Guid> CreateAsync(
        [Body] CreateCardDto command,
        [Header("Authorization")] string authorization
    );

    [Get("/api/cards")]
    Task<IEnumerable<CardDto>> GetAll([Header("Authorization")] string authorization);

    [Put("/api/cards/{id}")]
    Task UpdateAsync(
        Guid id,
        [Body] UpdateCardDto command,
        [Header("Authorization")] string authorization
    );
}