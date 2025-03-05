namespace Domain.Dtos.State;

public class CreateStateDto
{
    public string Description { get; set; }
    public Status Status { get; set; }
    public Priority Priority { get; set; }
    public Guid CardId { get; set; }
}