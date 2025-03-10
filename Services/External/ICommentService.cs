using Domain.Dtos.Comment;
using Refit;

namespace Services.External;

[Headers("Authorization: Bearer")]
public interface ICommentService
{
    [Get("/api/Comments/{cardId}")]
    Task<IEnumerable<CommentDto>> GetByCardIdAsync(Guid cardId, [Header("Authorization")] string authorization);

    [Post("/api/Comments")]
    Task<CommentDto> CreateAsync([Body] CreateCommentDto command, [Header("Authorization")] string authorization);

    [Put("/api/Comments/{id}")]
    Task UpdateAsync(Guid id, [Body] UpdateCommentDto command);

    [Delete("/api/Comments/{id}")]
    Task DeleteAsync(Guid id, [Header("Authorization")] string authorization);
}