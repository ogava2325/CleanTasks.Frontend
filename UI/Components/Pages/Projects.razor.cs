using Blazorise;
using Domain.Dtos.Project;
using Microsoft.AspNetCore.Components;
using Services.External;
using UI.Services;
using Modal = Blazorise.Modal;

namespace UI.Components.Pages;

public partial class Projects : ComponentBase
{
    [Inject] private IProjectService ProjectService { get; set; }
    [Inject] private CustomAuthStateProvider AuthStateProvider { get; set; }

    private List<ProjectDto> UserProjects { get; set; } = [];
    private CreateProjectDto _newProject = new();

    private Modal? _modalRef;

    Validations fluentValidation;

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
        // Reset form
        _newProject = new CreateProjectDto();

        if (_modalRef is not null)
        {
            await _modalRef.Show();
        }
    }

    private async Task HideModal()
    {
        if (_modalRef is not null)
        {
            await _modalRef.Hide();
        }
    }

    private async Task CreateProject()
    {
        if (await fluentValidation.ValidateAll())
        {
            try
            {
                _newProject.UserId = await AuthStateProvider.GetUserIdAsync();
                _newProject.RoleId = Guid.Parse("E9011C0D-111E-46C0-9CFC-3E1C9B043804");

                await ProjectService.CreateAsync(_newProject);
                await LoadProjects();
                await HideModal();
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error creating project: {exception.Message}");
            }
        }
    }
}