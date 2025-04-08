using Blazorise;
using Microsoft.AspNetCore.Components;

namespace UI.Components.Kanban;

public partial class OffCanvasMenu : ComponentBase
{
    private Offcanvas OffcanvasRef;
    private OffcanvasPage _currentPage = OffcanvasPage.Main;

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