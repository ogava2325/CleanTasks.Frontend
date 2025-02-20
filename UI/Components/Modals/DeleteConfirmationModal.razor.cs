using Microsoft.AspNetCore.Components;

namespace UI.Components.Modals;

public partial class DeleteConfirmationModal : ComponentBase
{
    [Parameter] public string Title { get; set; }
    [Parameter] public string Message { get; set; }
    [Parameter] public EventCallback<bool> OnConfirm { get; set; }

    private bool IsVisible { get; set; }

    public Task Show()
    {
        IsVisible = true;
        return Task.CompletedTask;
    }

    public Task Hide()
    {
        IsVisible = false;
        return Task.CompletedTask;
    }

    private async Task Confirm()
    {
        await Hide();
        await OnConfirm.InvokeAsync(true);
    }

    private async Task CloseModal()
    {
        await Hide();
        await OnConfirm.InvokeAsync(false);
    }
}