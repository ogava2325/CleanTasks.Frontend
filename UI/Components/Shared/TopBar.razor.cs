using Microsoft.AspNetCore.Components;
using UI.Services;

namespace UI.Components.Shared;

public partial class TopBar : ComponentBase
{

    [Inject] private NavigationManager Navigation { get; set; }
    [Inject] private CustomAuthStateProvider AuthStateProvider { get; set; }
    

    private async Task Logout()
    {
        await AuthStateProvider.Logout();
    }
}