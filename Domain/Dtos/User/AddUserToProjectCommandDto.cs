namespace Domain.Dtos.User;

public class AddUserToProjectCommandDto
{
    public string Email { get; set; }
    public Guid ProjectId { get; set; }
    public string Role { get; set; }
}