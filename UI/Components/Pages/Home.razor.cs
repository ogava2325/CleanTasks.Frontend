using Domain.Dtos.Stats;
using Microsoft.AspNetCore.Components;
using NewsAPI;
using NewsAPI.Constants;
using NewsAPI.Models;
using services.External;
using UI.Services;

namespace UI.Components.Pages;

public partial class Home : ComponentBase
{
    [Inject] public IStatsService StatsService { get; set; }
    [Inject] public IConfiguration Configuration { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    [Inject] public CustomAuthStateProvider AuthStateProvider { get; set; }
    
    private StatsDto Stats { get; set; } = new();
    private List<Article> Articles { get; set; } = [];
    
    private bool _isNewsLoading = true;
    
    protected override async Task OnInitializedAsync()
    {
        await LoadStatsAsync();
        await LoadNewsAsync();
    }

    private async Task LoadStatsAsync()
    {
        try
        {
            var currentUserId = await AuthStateProvider.GetUserIdAsync();
            Stats = await StatsService.GetAsync(currentUserId);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error loading stats: {e.Message}");
        }
    }

    private async Task LoadNewsAsync()
    {
        try
        {
            _isNewsLoading = true;

            var apiKey = Configuration["NewsApi:ApiKey"];
            var newsApiClient = new NewsApiClient(apiKey);
            var response = await newsApiClient.GetEverythingAsync(new EverythingRequest()
            {
                Q = "blazor",
                SortBy = SortBys.Relevancy,
                Language = Languages.EN,
                Page = 1,
                PageSize = 3
            });
            
            Articles = response.Articles;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading news: {ex.Message}");
        }
        finally
        {
            _isNewsLoading = false;
        }
    }

    private void OpenArticle(string url)
    {
        NavigationManager.NavigateTo(url, forceLoad: true);
    }
    
    private string FormatCount(int count, string label)
    {
        return count == 1 ? $"1 {label}" : $"{count} {label}s";
    }
    
    private string CreatedProjectsCountText => FormatCount(Stats.ProjectsCreatedCount, "Project");
    private string ProjectsMemberCountText => FormatCount(Stats.ProjectsMemberCount, "Project");
    private string CardsCountText => FormatCount(Stats.CardsCreatedCount, "Card");

    private string CreatedProjectPlural => Stats.ProjectsCreatedCount == 1 ? "Project" : "Projects";
    private string ProjectMemberPlural => Stats.ProjectsMemberCount == 1 ? "Project" : "Projects";
    private string CardPlural => Stats.CardsCreatedCount == 1 ? "Task" : "Tasks";
}