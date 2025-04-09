using Domain.Dtos.Stats;
using Refit;

namespace services.External;

public interface IStatsService
{
    [Get("/api/stats/{userId}")]
    Task<StatsDto> GetAsync(Guid userId);
}