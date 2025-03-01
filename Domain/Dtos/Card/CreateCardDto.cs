namespace Domain.Dtos.Card;

public class CreateCardDto
{
    public string Title { get; set; }
    
    public Guid ColumnId { get; set; }
    
    public int RowIndex { get; set; }
}