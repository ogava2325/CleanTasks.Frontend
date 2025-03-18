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

    private Validations _fluentValidation;

    readonly LoginUserDto _loginModel = new();

    private ErrorResponse? _errorResponse = new();
    
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

                await AuthStateProvider.Login(token, _loginModel.RememberMe);
                NavigationManager.NavigateTo("/");
            }
            catch (ApiException apiEx)
            {
                _errorResponse = await apiEx.GetContentAsAsync<ErrorResponse>();
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occured during login {e.Message}");
            }
        }
    }
}

public class ErrorResponse
{
    public string message { get; set; } = string.Empty;
}