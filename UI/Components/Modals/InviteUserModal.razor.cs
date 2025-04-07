using Blazorise;
using Domain.Dtos.User;
using Microsoft.AspNetCore.Components;
using services.External;
using UI.Services;

namespace UI.Components.Modals;

public partial class InviteUserModal : ComponentBase
{
    private Modal ModalRef;
    
    [Parameter] public EventCallback OnCancelClicked { get; set; }
    [Parameter] public EventCallback<Guid> OnInviteClicked { get; set; }
    [Inject] public IUserService UserService { get; set; } = default!;
    [Inject] public CustomAuthStateProvider AuthStateProvider { get; set; } = default!;

    public Guid CurrentUserId { get; set; }

    public List<UserDto> Users { get; set; } = [];

    public Guid SelectedUserId { get; set; } = Guid.NewGuid();

    protected override async Task OnInitializedAsync()
    {
        CurrentUserId = await AuthStateProvider.GetUserIdAsync();
        Users = (await UserService.GetAll()).Where(u => u.Id != CurrentUserId).ToList();
    }

    public async Task Show()
    {
        await ModalRef.Show();
    }

    public async Task Hide()
    {
        await ModalRef.Hide();
    }

    private async Task OnCancel()
    {
        await Hide();
        if (OnCancelClicked.HasDelegate)
        {
            await OnCancelClicked.InvokeAsync();
        }
    }
    
    private async Task OnInvite()
    {
        await Hide();
        if (OnInviteClicked.HasDelegate)
        {
            await OnInviteClicked.InvokeAsync(SelectedUserId);
        }
    }
}