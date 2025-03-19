using Blazorise;
using Domain.Dtos.User;
using Microsoft.AspNetCore.Components;
using Services.External;
using UI.Services;

namespace UI.Components.Pages;

public partial class Register : ComponentBase
{
    [Inject] private CustomAuthStateProvider AuthStateProvider { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; }
    [Inject] private IUserService UserService { get; set; }

    private readonly RegisterUserDto _registerModel = new();
    
    private string? _errorResponse;

    private async void OnRegister()
    {
        try
        {
            var response = await UserService.RegisterAsync(new RegisterUserDto()
            {
                FirstName = _registerModel.FirstName,
                LastName = _registerModel.LastName,
                Email = _registerModel.Email,
                Password = _registerModel.Password,
            });

            if (response.IsSuccess)
            {
                NavigationManager.NavigateTo("/login");
            }
            else
            {
                _errorResponse = response.Error;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occured during registration: {ex.Message}");
        }
    }
}