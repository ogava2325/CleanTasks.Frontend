using Blazorise;
using Microsoft.AspNetCore.Components;

namespace UI.Components;

public partial class CommentDeleteConfirmationModal : ComponentBase
{
    [Parameter] public string Title { get; set; } = "Confirm Delete";
    [Parameter] public string Message { get; set; } = "Are you sure you want to delete this item?";
    [Parameter] public string ConfirmText { get; set; } = "Delete";
    [Parameter] public string CancelText { get; set; } = "Cancel";
    [Parameter] public ModalSize ModalSize { get; set; } = ModalSize.Default;

    [Parameter] public EventCallback OnConfirmed { get; set; }
    [Parameter] public EventCallback OnCancelled { get; set; }

    public bool IsVisible { get; set; }

    public void Show() => IsVisible = true;

    private async Task Confirm()
    {
        IsVisible = false;
        await OnConfirmed.InvokeAsync();
    }

    private async Task Cancel()
    {
        IsVisible = false;
        await OnCancelled.InvokeAsync();
    }
}