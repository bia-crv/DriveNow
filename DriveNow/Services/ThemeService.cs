namespace DriveNow.Services;

public class ThemeService
{
    public bool IsDark { get; private set; }

    public event Action? OnChange;

    public void Toggle()
    {
        IsDark = !IsDark;
        OnChange?.Invoke();
    }
}