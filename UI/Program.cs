using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.FluentValidation;
using Blazorise.Icons.FontAwesome;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using services.Extensions;
using UI.Components;
using UI.Services;

var builder = WebApplication.CreateBuilder(args);

// Services for authentication and authorization
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => { options.LoginPath = "/login"; });

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<ProtectedSessionStorage>();
builder.Services.AddScoped<ProtectedLocalStorage>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<CustomAuthStateProvider>());
builder.Services.AddScoped<CustomAuthStateProvider>();

// Services for Refit
builder.Services.AddRefitServices(builder.Configuration);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Blazorise library
builder.Services
    .AddBlazorise(options => { options.Immediate = true; })
    .AddBootstrap5Providers()
    .AddFontAwesomeIcons()
    .AddBlazoriseFluentValidation();

builder.Services.AddValidatorsFromAssembly(typeof(App).Assembly);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();