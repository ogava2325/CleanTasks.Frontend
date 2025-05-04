using System.Net;
using Blazorise;
using Domain.Dtos.Project;
using Domain.Dtos.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Refit;
using services.External;
using UI.Components.Modals;
using UI.Services;

namespace UI.Components.Pages;

public partial class Projects : ComponentBase, IAsyncDisposable
{
    [Inject] private IProjectService ProjectService { get; set; } = default!;
    [Inject] private CustomAuthStateProvider AuthStateProvider { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ILogger<Projects> Logger { get; set; } = default!;
    [Inject] private IConfiguration Configuration { get; set; } = default!;
    [Inject] private INotificationService NotificationService { get; set; } = default!;
    private PaginatedList<ProjectDto>? PaginatedProjectsList { get; set; }

    private CreateProjectDto NewProject { get; set; } = new();
    private CreateProjectModal ProjectModalRef { get; set; } = default!;

    private int PageSize { get; set; } = 11;
    private int PageNumber { get; set; } = 1;

    private DateTimeOffset? StartDate { get; set; }
    private DateTimeOffset? EndDate { get; set; }
    
    private HubConnection HubConnection { get; set; } = default!;
    
    private ProjectSortOption _selectedSortOption = ProjectSortOption.CreatedDesc;

    private ProjectSortOption SelectedSortOption
    {
        get => _selectedSortOption;
        set
        {
            if (_selectedSortOption == value) return;
            _selectedSortOption = value;
            _ = LoadProjectsAsync();
        }
    }

    private ProjectsSortBy SortBy { get; set; } = ProjectsSortBy.CreatedAtUtc;
    private ProjectsSortOrder SortOrder { get; set; } = ProjectsSortOrder.Desc;

    private string? _searchTerm;

    private string? SearchTerm
    {
        get => _searchTerm;
        set
        {
            if (_searchTerm == value) return;
            _searchTerm = value;
            PageNumber = 1;
            _ = LoadProjectsAsync();
        }
    }

    private static readonly Dictionary<ProjectSortOption, (ProjectsSortBy, ProjectsSortOrder)> SortMapping = new()
    {
        { ProjectSortOption.NameAsc, (ProjectsSortBy.Title, ProjectsSortOrder.Asc) },
        { ProjectSortOption.NameDesc, (ProjectsSortBy.Title, ProjectsSortOrder.Desc) },
        { ProjectSortOption.CreatedAsc, (ProjectsSortBy.CreatedAtUtc, ProjectsSortOrder.Asc) },
        { ProjectSortOption.CreatedDesc, (ProjectsSortBy.CreatedAtUtc, ProjectsSortOrder.Desc) }
    };

    protected override async Task OnInitializedAsync()
    {
        await LoadProjectsAsync();
        await InitializeSignalRAsync();
    }

    private async Task InitializeSignalRAsync()
    {
        var token = await AuthStateProvider.GetToken();
        var hubUrl = Configuration["HubUrl"];
        
        HubConnection = new HubConnectionBuilder()
            .WithUrl(hubUrl, options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(token)!;
            })
            .WithAutomaticReconnect()
            .Build();
        
        await HubConnection.StartAsync();
        
        HubConnection.On<Guid>("UserAdded", async (projectId) =>
        {
            await InvokeAsync(async () =>
            {
                await LoadProjectsAsync();
                StateHasChanged();
            });
        });
        
        HubConnection.On<Guid>("ProjectDeleted", async (projectId) =>
        {
            await InvokeAsync(async () =>
            {
                await LoadProjectsAsync();
                StateHasChanged();
            });
        });
        
        HubConnection.On<Guid>("ProjectUpdated", async (projectId) =>
        {
            await InvokeAsync(async () =>
            {
                await LoadProjectsAsync();
                StateHasChanged();
            });
        });
        
        HubConnection.On<Guid, Guid>("UserRemoved", async (projectId, userId) =>
        {
            await InvokeAsync(async () =>
            {
                await LoadProjectsAsync();
                StateHasChanged();
            });
        });
    }
    
    private async Task LoadProjectsAsync()
    {
        try
        {
            var userId = await AuthStateProvider.GetUserIdAsync();
            var (sortBy, sortOrder) = SortMapping[SelectedSortOption];

            var adjustedEndDate = EndDate?.Date.AddDays(1).AddSeconds(-1);
            
            var token = await AuthStateProvider.GetToken();
            
            var paginationParameters = new PaginationParameters
            {
                PageNumber = PageNumber,
                PageSize = PageSize,
                SearchTerm = SearchTerm,
                SortBy = sortBy,
                SortOrder = sortOrder
            };

            PaginatedProjectsList = await ProjectService.GetByUserId(
                userId,
                paginationParameters,
                StartDate,
                adjustedEndDate,
                false,
                $"Bearer {token}"
            );
            
            StateHasChanged();
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Error loading projects: {exception.Message}");
        }
    }

    private async Task ShowCreateProjectModal()
    {
        NewProject = new CreateProjectDto();

        await ProjectModalRef.Show();
    }

    private async Task HideCreateProjectModal()
    {
        await ProjectModalRef.Hide();
    }

    private async Task CreateProject()
    {
        try
        {
            NewProject.UserId = await AuthStateProvider.GetUserIdAsync();

            var token = await AuthStateProvider.GetToken();
            await ProjectService.CreateAsync(NewProject, $"Bearer {token}");
            await LoadProjectsAsync();
            await HideCreateProjectModal();
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Error creating project: {exception.Message}");
        }
    }

    private async Task DeleteProject(Guid projectId)
    {
        var token = await AuthStateProvider.GetToken();
        try
        {
            await ProjectService.DeleteAsync(projectId, $"Bearer {token}");
            await ShowSuccessNotification("Project deleted successfully.");
        }
        catch (ApiException e)
        {
            if (e.StatusCode == HttpStatusCode.Forbidden)
            {
                await ShowErrorNotification("You do not have permission to delete this project.");
            }
        }
        finally
        {
            await LoadProjectsAsync();
            StateHasChanged();
        }
    }

    private async Task GoToPreviousPage()
    {
        if (PaginatedProjectsList?.HasPreviousPage == true)
        {
            PageNumber--;
            await LoadProjectsAsync();
        }
    }

    private async Task GoToNextPage()
    {
        if (PaginatedProjectsList?.HasNextPage == true)
        {
            PageNumber++;
            await LoadProjectsAsync();
        }
    }

    private async Task SetActivePage(string page)
    {
        PageNumber = int.Parse(page);
        await LoadProjectsAsync();
    }

    private async Task ClearFiltersAsync()
    {
        StartDate = null;
        EndDate = null;
        await LoadProjectsAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await HubConnection.DisposeAsync();
    }
    
    private Task ShowErrorNotification(string message)
    {
        return NotificationService.Error(message);
    }
    
    private Task ShowSuccessNotification(string message)
    {
        return NotificationService.Success(message);
    }
}