using System.Text.Json.Serialization;

namespace Domain.Dtos.State;

public class UpdateStateDto
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter<Status>))]
    public Status Status { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter<Priority>))]
    public Priority Priority { get; set; }
    public Guid CardId { get; set; }
}