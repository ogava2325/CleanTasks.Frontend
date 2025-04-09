using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Services.Auth;

namespace UI.Services;

public class CustomAuthStateProvider(
    NavigationManager navigation, 
    ProtectedLocalStorage localStorage,
    ProtectedSessionStorage sessionStorage)
    : AuthenticationStateProvider
{
    private ClaimsPrincipal _currentUser = new ClaimsPrincipal(new ClaimsIdentity());

    private const string TokenKey = "AuthToken";

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        // First check local storage (for "remember me")
        var localResult = await localStorage.GetAsync<string>(TokenKey);
        if (localResult.Success && !string.IsNullOrEmpty(localResult.Value))
        {
            var identity = new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(localResult.Value), "jwt");
            _currentUser = new ClaimsPrincipal(identity);
            return new AuthenticationState(_currentUser);
        }
        
        // Then check session storage
        var sessionResult = await sessionStorage.GetAsync<string>(TokenKey);
        if (sessionResult.Success && !string.IsNullOrEmpty(sessionResult.Value))
        {
            var identity = new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(sessionResult.Value), "jwt");
            _currentUser = new ClaimsPrincipal(identity);
            return new AuthenticationState(_currentUser);
        }
        
        return new AuthenticationState(_currentUser);
    }

    public async Task Login(string token, bool rememberMe)
    {
        if (rememberMe)
        {
            await localStorage.SetAsync(TokenKey, token);
        }
        else
        {
            await sessionStorage.SetAsync(TokenKey, token);
        }
        
        var identity = new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "jwt");
        _currentUser = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
    }

    public async Task Logout()
    {
        await sessionStorage.DeleteAsync(TokenKey);
        await localStorage.DeleteAsync(TokenKey);
        _currentUser = new ClaimsPrincipal(new ClaimsIdentity());

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        navigation.NavigateTo("login");
    }

    public async Task<string> GetToken()
    {
        var localResult = await localStorage.GetAsync<string>(TokenKey);
        if (localResult.Success && !string.IsNullOrEmpty(localResult.Value))
        {
            return localResult.Value;
        }
        
        var sessionResult = await sessionStorage.GetAsync<string>(TokenKey);

        if (!sessionResult.Success || string.IsNullOrEmpty(sessionResult.Value))
        {
            return string.Empty;
        }

        return sessionResult.Value;
    }

    public async Task<Guid> GetUserIdAsync()
    {
        var token = await GetToken();

        var claims = JwtParser.ParseClaimsFromJwt(token);
        var userIdString = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        return Guid.TryParse(userIdString, out var userId) ? userId : Guid.Empty;
    }

    public async Task<UserInfo> GetUserAsync()
    {
        var token = await GetToken();
        if (string.IsNullOrEmpty(token))
        {
            return new UserInfo();
        }

        var claims = JwtParser.ParseClaimsFromJwt(token);

        return new UserInfo
        {
            Id = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value,
            Name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "User",
            Email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? "Unknown"
        };
    }
}

public class UserInfo
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string Email { get; set; }
}