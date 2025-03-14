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

    public async Task<Guid> GetUserIdAsync()
    {
        var sessionResult = await sessionStorage.GetAsync<string>(TokenKey);

        if (!sessionResult.Success || string.IsNullOrEmpty(sessionResult.Value))
        {
            return Guid.Empty;
        }

        var claims = JwtParser.ParseClaimsFromJwt(sessionResult.Value);
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