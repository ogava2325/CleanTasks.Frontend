using Domain.Dtos.Project;
using Domain.Dtos.Shared;
using Microsoft.AspNetCore.Components;
using services.External;
using UI.Components.Modals;
using UI.Services;

namespace UI.Components.Pages;

public partial class Projects : ComponentBase
{
    [Inject] private IProjectService ProjectService { get; set; } = default!;
    [Inject] private CustomAuthStateProvider AuthStateProvider { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    private PaginatedList<ProjectDto>? PaginatedProjectsList { get; set; }

    private CreateProjectDto NewProject { get; set; } = new();
    private CreateProjectModal ProjectModalRef { get; set; } = default!;

    private int PageSize { get; set; } = 11;
    private int PageNumber { get; set; } = 1;

    private DateTimeOffset? StartDate { get; set; }
    private DateTimeOffset? EndDate { get; set; }


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
    }

    private async Task LoadProjectsAsync()
    {
        try
        {
            var userId = await AuthStateProvider.GetUserIdAsync();
            var (sortBy, sortOrder) = SortMapping[SelectedSortOption];

            var adjustedEndDate = EndDate?.Date.AddDays(1).AddSeconds(-1);
            
            var token = await AuthStateProvider.GetToken();

            PaginatedProjectsList = await ProjectService.GetByUserId(
                userId,
                PageNumber,
                PageSize,
                SearchTerm,
                sortBy,
                sortOrder,
                StartDate,
                adjustedEndDate,
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
        try
        {
            var token = await AuthStateProvider.GetToken();
            
            await ProjectService.DeleteAsync(projectId, $"Bearer {token}");
            await LoadProjectsAsync();
            StateHasChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting project: {ex.Message}");
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
}