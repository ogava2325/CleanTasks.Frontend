using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace UI.Services;

public class AuthHttpClientHandler : DelegatingHandler
{
    private readonly CustomAuthStateProvider _authStateProvider;

    public AuthHttpClientHandler(CustomAuthStateProvider authStateProvider)
    {
        _authStateProvider = authStateProvider;
    }
    
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Retrieve the token from session storage
        var token = await _authStateProvider.GetToken();
        
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}