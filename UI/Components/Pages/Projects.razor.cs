using Domain.Dtos.Project;
using Microsoft.AspNetCore.Components;
using Services.External;
using UI.Components.Modals;
using UI.Services;

namespace UI.Components.Pages;

public partial class Projects : ComponentBase
{
    [Inject] private IProjectService ProjectService { get; set; }
    [Inject] private CustomAuthStateProvider AuthStateProvider { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; }

    private List<ProjectDto> UserProjects { get; set; } = [];
    private CreateProjectDto _newProject = new();

    private CreateProjectModal _projectModalRef;

    protected override async Task OnInitializedAsync()
    {
        await LoadProjects();
    }

    private async Task LoadProjects()
    {
        try
        {
            var userId = await AuthStateProvider.GetUserIdAsync();
            UserProjects = (await ProjectService.GetByUserId(userId)).ToList();
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Error loading projects: {exception.Message}");
        }
    }

    private async Task ShowModal()
    {
        _newProject = new CreateProjectDto();

        await _projectModalRef.Show();
    }

    private async Task HideModal()
    {
        await _projectModalRef.Hide();
    }

    private async Task CreateProject()
    {
        try
        {
            _newProject.UserId = await AuthStateProvider.GetUserIdAsync();
            _newProject.RoleId = Guid.Parse("18C7423E-F36B-1410-8ED2-0074E3FF4145");

            await ProjectService.CreateAsync(_newProject);
            await LoadProjects();
            await HideModal();
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
            await ProjectService.DeleteAsync(projectId);
            UserProjects.RemoveAll(p => p.Id == projectId);
            StateHasChanged(); 
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting project: {ex.Message}");
        }
    }
}