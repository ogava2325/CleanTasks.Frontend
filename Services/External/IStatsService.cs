using Domain.Dtos.Stats;
using Refit;

namespace Services.External;

public interface IStatsService
{
    [Get("/api/stats")]
    Task<StatsDto> GetAsync();
}