namespace Domain.Dtos.Column;

public class ColumnDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    
    public Guid ProjectId { get; set; }
    
    public DateTimeOffset CreatedAtUtc { get; set; }
}