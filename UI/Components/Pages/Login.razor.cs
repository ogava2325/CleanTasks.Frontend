using System.Net;
using Blazorise;
using Domain.Dtos.User;
using Microsoft.AspNetCore.Components;
using Refit;
using Services.External;
using UI.Services;

namespace UI.Components.Pages;

public partial class Login : ComponentBase
{
    [Inject] private CustomAuthStateProvider AuthStateProvider { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; }
    [Inject] private IUserService UserService { get; set; }
    
    readonly LoginUserDto _loginModel = new();

    private string? _errorResponse;

    private async Task OnLogin()
    {
        try
        {
            var response = await UserService.LoginAsync(new LoginUserDto()
            {
                Email = _loginModel.Email,
                Password = _loginModel.Password
            });

            if (response.IsSuccess)
            {
                await AuthStateProvider.Login(response.Value, _loginModel.RememberMe);
                NavigationManager.NavigateTo("/");
            }
            else
            {
                _errorResponse = response.Error;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occured during login {e.Message}");
        }
    }
}