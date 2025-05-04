namespace Domain.Dtos.User;

public class RemoveUserFromProjectDto
{
    public Guid UserId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid CurrentUserId { get; set; }
}