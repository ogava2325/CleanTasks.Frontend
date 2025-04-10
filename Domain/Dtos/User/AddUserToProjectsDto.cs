namespace Domain.Dtos.User;

public class AddUserToProjectsDto
{
    public Guid UserId { get; set; }
    public Guid ProjectId { get; set; }
}