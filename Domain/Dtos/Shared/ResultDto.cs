namespace Domain.Dtos.Shared;

using System.Text.Json.Serialization;

public class ResultDto<T>
{
    [JsonPropertyName("value")]
    public T Value { get; set; }

    [JsonPropertyName("isSuccess")]
    public bool IsSuccess { get; set; }

    [JsonPropertyName("isFailure")]
    public bool IsFailure { get; set; }

    [JsonPropertyName("error")]
    public string Error { get; set; }
}
