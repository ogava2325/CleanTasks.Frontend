using Blazorise;
using Domain.Dtos.User;
using Microsoft.AspNetCore.Components;
using Services.External;
using UI.Services;

namespace UI.Components.Pages;

public partial class Login : ComponentBase
{
    [Inject] private CustomAuthStateProvider AuthStateProvider { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; }
    [Inject] private IUserService UserService { get; set; }

    private Validations _fluentValidation;

    readonly LoginUserDto _loginModel = new();
    
    private async Task OnLogin()
    {
        if (await _fluentValidation.ValidateAll())
        {
            try
            {
                var token = await UserService.LoginAsync(new LoginUserDto()
                {
                    Email = _loginModel.Email,
                    Password = _loginModel.Password
                });
            
                await AuthStateProvider.Login(token);
                NavigationManager.NavigateTo("/");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
            }
        }
    }
}