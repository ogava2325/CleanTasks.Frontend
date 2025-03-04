using Blazored.TextEditor;
using Blazorise.Bootstrap5.Components;
using Domain.Dtos.State;
using Microsoft.AspNetCore.Components;
using Services.External;
using Modal = Blazorise.Modal;

namespace UI.Components.Modals;

public partial class CardDetailsModal : ComponentBase
{
    [Inject] public IStateService StateService { get; set; }
    [Parameter] public Guid CardId { get; set; }
    [Parameter] public EventCallback OnCancelClicked { get; set; }
    
    private Modal ModalRef { get; set; }
    private BlazoredTextEditor QuillHtml { get; set; }
    
    private StateDto State { get; set; } = new ()
    {
        Id = new Guid(),
        CardId = new Guid(),
        Description = string.Empty,
        Priority = Priority.Low,
        Status = Status.Pending
    };
    
    private UpdateStateDto _updateState = new();
    
    public async Task ShowAsync()
    {
        await ModalRef.Show();

        try
        {
            State = await StateService.GetByCardIdAsync(CardId);
            await QuillHtml.LoadHTMLContent(State.Description);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public async Task Hide()
    {
        await ModalRef.Hide();
    }

    private async Task UpdateStateAsync()
    {
        var updateState = new UpdateStateDto
        {
            Id = State.Id,
            CardId = CardId,
            Status = State.Status,
            Priority = State.Priority,
            Description = await QuillHtml.GetHTML()
        };
        try
        {
            await StateService.UpdateAsync(CardId, updateState);
            await Hide();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error updating state: {e.Message}");
        }
    }

    private async Task OnCancelAsync()
    {
        await Hide();

        if (OnCancelClicked.HasDelegate)
        {
            await OnCancelClicked.InvokeAsync();
        }
    }
}