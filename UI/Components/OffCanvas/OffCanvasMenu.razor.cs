using System.Net;
using Blazorise;
using Domain.Dtos.Project;
using Microsoft.AspNetCore.Components;
using Refit;
using services.External;
using UI.Services;

namespace UI.Components.OffCanvas;

public partial class OffCanvasMenu : ComponentBase
{
    [Parameter] public ProjectDto CurrentProject { get; set; } = new();
    [Parameter] public EventCallback OnProjectArchive { get; set; }
    [Parameter] public EventCallback OnProjectRestore { get; set; }
    [Inject] private IProjectService ProjectService { get; set; } = default!;
    [Inject] private CustomAuthStateProvider AuthStateProvider { get; set; } = default!;
    [Inject] private INotificationService NotificationService { get; set; } = default!;
    private Offcanvas OffcanvasRef { get; set; } = default!;

    private OffcanvasPage _currentPage = OffcanvasPage.Main;

    public bool ShowCloseProjectPanel { get; set; } = false;

    private string GetHeaderTitle() =>
        _currentPage switch
        {
            OffcanvasPage.Main => "Menu",
            OffcanvasPage.About => "About Project",
            OffcanvasPage.Activity => "Activity",
            OffcanvasPage.Labels => "Labels",
            _ => "Menu"
        };

    public async Task HideOffcanvasAsync()
    {
        await OffcanvasRef.Hide();
    }

    public async Task ShowOffcanvasAsync()
    {
        await OffcanvasRef.Show();
    }

    private async Task ConfirmCloseProjectAsync()
    {
        var token = await AuthStateProvider.GetToken();
        try
        {
            await ProjectService.ArchiveAsync(CurrentProject.Id, $"Bearer {token}");
        }
        catch (ApiException e)
        {
            if (e.StatusCode == HttpStatusCode.Forbidden)
            {
                await ShowErrorNotification("You do not have permission to archive this project.");
            }
            Console.WriteLine($"Error deleting project: {e.Message}");
        }


        await OnProjectArchive.InvokeAsync();
        ShowCloseProjectPanel = false;
    }

    private void ToggleCloseProjectPanel()
    {
        ShowCloseProjectPanel = !ShowCloseProjectPanel;
    }

    private async Task RestoreProjectAsync()
    {
        var token = await AuthStateProvider.GetToken();
        try
        {
            await ProjectService.RestoreAsync(CurrentProject.Id, $"Bearer {token}");
        }
        catch(ApiException e)
        {
            if(e.StatusCode == HttpStatusCode.Forbidden)
            {
                await ShowErrorNotification("You do not have permission to restore this project.");
            }
        }

        await OnProjectRestore.InvokeAsync();
    }
    
    private Task ShowErrorNotification(string message)
    {
        return NotificationService.Error(message);
    }
}