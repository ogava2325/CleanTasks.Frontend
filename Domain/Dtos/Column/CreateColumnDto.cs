namespace Domain.Dtos.Column;

public class CreateColumnDto
{
    public string Title { get; set; }
    public Guid ProjectId { get; set; }
}