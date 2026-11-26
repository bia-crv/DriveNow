using DriveNow.Components;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);
var apiBaseAddress = new Uri("https://egolance.com/api/");
builder.Services.AddHttpClient<DriveNow.Services.CarService>(c => c.BaseAddress = apiBaseAddress);
builder.Services.AddHttpClient<DriveNow.Services.BookingService>(c => c.BaseAddress = apiBaseAddress);
builder.Services.AddHttpClient<DriveNow.Services.LocationService>(c => c.BaseAddress = apiBaseAddress);

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<DriveNow.Services.DriveNowAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<DriveNow.Services.DriveNowAuthStateProvider>());
builder.Services.AddScoped<DriveNow.Services.AuthService>();
// Add services to the container.
builder.Services.AddRazorComponents()
    
    .AddInteractiveServerComponents();

builder.Services.AddScoped<DriveNow.Services.ThemeService>();
builder.Services.AddScoped<DriveNow.Services.FleetStateService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();