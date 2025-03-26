using System.Text.Json.Serialization;

namespace Domain.Dtos.Shared;

public class PaginatedList<T>
{
    public PaginatedList(List<T> items, int count, int pageNumber, int pageSize)
    {
        Items = items;
        PageNumber = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        TotalCount = count;
    }
    
    public PaginatedList() { }

    [JsonPropertyName("items")]
    public List<T> Items { get; set; }
    
    [JsonPropertyName("pageNumber")]
    public int PageNumber { get; set; }
    
    [JsonPropertyName("totalPages")]
    public int TotalPages { get; set; }
    
    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; }
        
    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }
    
    [JsonPropertyName("hasPreviousPage")]
    public bool HasPreviousPage { get; set; }
    
    [JsonPropertyName("hasNextPage")]
    public bool HasNextPage { get; set; }
}