using DriveNow.Models;

namespace DriveNow.Services;

public class AuthService
{
    private static readonly List<(string Username, string Password, string DisplayName, string Role)> Users = new()
    {
        ("admin", "admin123", "Bianca", "Admin"),
        ("staff", "staff123", "Staff User", "Staff")
    };

    private readonly DriveNowAuthStateProvider _authProvider;

    public AuthService(DriveNowAuthStateProvider authProvider)
    {
        _authProvider = authProvider;
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        var user = Users.FirstOrDefault(u =>
            u.Username.Equals(username.Trim(), StringComparison.OrdinalIgnoreCase) &&
            u.Password == password);

        if (user.Username is null)
            return false;

        await _authProvider.SignInAsync(new UserSession
        {
            Username = user.Username,
            DisplayName = user.DisplayName,
            Role = user.Role
        });

        return true;
    }

    public Task LogoutAsync() => _authProvider.SignOutAsync();
}