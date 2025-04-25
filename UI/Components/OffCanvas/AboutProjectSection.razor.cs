using System.Net;
using Blazorise;
using Blazorise.RichTextEdit;
using Domain.Dtos.Project;
using Microsoft.AspNetCore.Components;
using Refit;
using services.External;
using UI.Services;

namespace UI.Components.OffCanvas;

public partial class AboutProjectSection : ComponentBase
{
    [Parameter] public ProjectDto CurrentProject { get; set; } = new();
    [Inject] public IProjectService ProjectService { get; set; } = default!;
    [Inject] public CustomAuthStateProvider AuthStateProvider { get; set; } = default!;
    [Inject] private INotificationService NotificationService { get; set; } = default!;
    
    private bool IsEditing { get; set; }

    private RichTextEdit richTextEdit;
    
    private async Task StartEditing()
    {
        IsEditing = true;
        
        StateHasChanged();
    }
    
    private async Task LoadProjectAsync()
    {
        try
        {
            var token = await AuthStateProvider.GetToken();
            CurrentProject = await ProjectService.GetById(CurrentProject.Id, $"Bearer {token}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error loading project: {e.Message}");
        }
    }
    private async Task SaveDescription()
    {
        var projectToUpdate = new UpdateProjectDto
        {
            Id = CurrentProject.Id,
            Title = CurrentProject.Title,
            Description = await richTextEdit.GetHtmlAsync(),
            UserId = await AuthStateProvider.GetUserIdAsync()
        };

        try
        {
            var token = await AuthStateProvider.GetToken();
            await ProjectService.UpdateAsync(CurrentProject.Id, projectToUpdate, $"Bearer {token}");
        }
        catch (ApiException e)
        {
            if (e.StatusCode == HttpStatusCode.Forbidden)
            {
                await ShowErrorNotification("You don't have permission to edit project's title.");
            }
            
            Console.WriteLine($"Error saving project description: {e.Message}");
        }
        finally
        {
            await LoadProjectAsync();
        }
        
        IsEditing = false;
    }
    
    private void CancelEditing()
    {
        IsEditing = false;
    }
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (richTextEdit != null)
        {
            await richTextEdit.SetHtmlAsync(CurrentProject.Description);
        }
    }
    
    private Task ShowErrorNotification(string message)
    {
        return NotificationService.Error(message);
    }
}