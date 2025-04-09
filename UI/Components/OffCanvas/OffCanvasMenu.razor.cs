using Blazored.TextEditor;
using Blazorise;
using Domain.Dtos.Project;
using Microsoft.AspNetCore.Components;

namespace UI.Components.OffCanvas;

public partial class OffCanvasMenu : ComponentBase
{
    [Parameter] public ProjectDto CurrentProject { get; set; } = new();
    private Offcanvas OffcanvasRef { get; set; } = default!;
    
    private OffcanvasPage _currentPage = OffcanvasPage.Main;

    public BlazoredTextEditor BlazoredTextEditor1 { get; set; }

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
}