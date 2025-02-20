namespace Domain.Dtos.Project;

public class CreateProjectDto
{
    public string Title { get; set; }
    
    public string Description { get; set; }
    
    public Guid UserId { get; set; }

    public Guid RoleId { get; set; }
}