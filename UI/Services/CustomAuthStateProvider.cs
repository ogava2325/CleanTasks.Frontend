using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Services;
using Services.Auth;

namespace UI.Services;

public class CustomAuthStateProvider(NavigationManager navigation, ProtectedSessionStorage sessionStorage)
    : AuthenticationStateProvider
{
    private ClaimsPrincipal _currentUser = new ClaimsPrincipal(new ClaimsIdentity());

    private const string TokenKey = "AuthToken";

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var sessionResult = await sessionStorage.GetAsync<string>(TokenKey);

        if (!sessionResult.Success || string.IsNullOrEmpty(sessionResult.Value))
        {
            return new AuthenticationState(_currentUser);
        }

        var identity = new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(sessionResult.Value), "jwt");
        _currentUser = new ClaimsPrincipal(identity);

        return new AuthenticationState(_currentUser);
    }

    public async Task Login(string token)
    {
        await sessionStorage.SetAsync(TokenKey, token);
        var identity = new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "jwt");
        _currentUser = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
    }

    public async Task Logout()
    {
        await sessionStorage.DeleteAsync(TokenKey);
        _currentUser = new ClaimsPrincipal(new ClaimsIdentity());

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        navigation.NavigateTo("login");
    }
    
    public async Task<string> GetToken()
    {
        var sessionResult = await sessionStorage.GetAsync<string>(TokenKey);

        if (!sessionResult.Success || string.IsNullOrEmpty(sessionResult.Value))
        {
            return string.Empty;
        }
        
        return sessionResult.Value;
    }
}