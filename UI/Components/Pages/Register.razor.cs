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

    private readonly RegisterUserDto registerModel = new();

    Validations fluentValidation;

    private async void OnRegister()
    {
        if (await fluentValidation.ValidateAll())
        {
            try
            {
                var user = new RegisterUserDto()
                {
                    FirstName = registerModel.FirstName,
                    LastName = registerModel.LastName,
                    Email = registerModel.Email,
                    Password = registerModel.Password,
                };

                await UserService.RegisterAsync(user);

                NavigationManager.NavigateTo("/login");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Register error: {ex.Message}");
            }
        }
    }
}