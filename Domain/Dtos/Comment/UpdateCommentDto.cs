namespace Domain.Dtos.Comment;

public class UpdateCommentDto
{
    public Guid Id { get; set; }
    public Guid CardId { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; }
}