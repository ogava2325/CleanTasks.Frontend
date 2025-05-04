namespace Domain.Dtos.Project;

public class ChangeUserRoleDto
{
    public Guid ProjectId { get; set; }
    public Guid UserId { get; set; }
    public string Role { get; set; }
}
