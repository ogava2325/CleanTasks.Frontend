using Domain.Dtos.Stats;
using Microsoft.AspNetCore.Components;
using NewsAPI;
using NewsAPI.Constants;
using NewsAPI.Models;
using Services.External;

namespace UI.Components.Pages;

public partial class Home : ComponentBase
{
    [Inject] public IStatsService StatsService { get; set; }
    [Inject] public IConfiguration Configuration { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    
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
            Stats = await StatsService.GetAsync();
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
    
    private MarkupString RedirectToLogin()
    {
        NavigationManager.NavigateTo("login", forceLoad: true);

        return (MarkupString)"<p>Redirecting to login...</p>";
    }
    
    private string FormatCount(int count, string label)
    {
        return count == 1 ? $"1 {label}" : $"{count} {label}s";
    }
    
    private string ProjectsCountText => FormatCount(Stats.ProjectsCount, "Project");
    private string CardsCountText => FormatCount(Stats.CardsCount, "Card");
    private string UsersCountText => FormatCount(Stats.UsersCount, "User");

    private string ProjectPlural => Stats.ProjectsCount == 1 ? "Project" : "Projects";
    private string TaskPlural => Stats.CardsCount == 1 ? "Task" : "Tasks";
    private string UserPlural => Stats.UsersCount == 1 ? "User" : "Users";
}