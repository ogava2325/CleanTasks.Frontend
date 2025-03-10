namespace Domain.Dtos.Comment;

public class CreateCommentDto
{
    public Guid CardId { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; }
}