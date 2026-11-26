using System.Security.Claims;
using DriveNow.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace DriveNow.Services;

public class DriveNowAuthStateProvider : AuthenticationStateProvider
{
    private const string StorageKey = "drivenow_user";
    private static readonly AuthenticationState Anonymous =
        new(new ClaimsPrincipal(new ClaimsIdentity()));

    private readonly ProtectedSessionStorage _storage;

    public DriveNowAuthStateProvider(ProtectedSessionStorage storage)
    {
        _storage = storage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var result = await _storage.GetAsync<UserSession>(StorageKey);
            if (result.Success && result.Value is not null)
                return new AuthenticationState(CreatePrincipal(result.Value));
        }
        catch
        {
            // Storage unavailable or data was tampered with: treat as logged out.
        }

        return Anonymous;
    }

    public async Task SignInAsync(UserSession session)
    {
        await _storage.SetAsync(StorageKey, session);
        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(CreatePrincipal(session))));
    }

    public async Task SignOutAsync()
    {
        await _storage.DeleteAsync(StorageKey);
        NotifyAuthenticationStateChanged(Task.FromResult(Anonymous));
    }

    private static ClaimsPrincipal CreatePrincipal(UserSession session)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, session.Username),
            new Claim("DisplayName", session.DisplayName),
            new Claim(ClaimTypes.Role, session.Role)
        };

        return new ClaimsPrincipal(new ClaimsIdentity(claims, "DriveNowAuth"));
    }
}