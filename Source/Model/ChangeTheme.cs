using Microsoft.Win32;

public class ChangeTheme
{
    private const string RegistryPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";

    public static void change(bool mode)
    {
        int value = mode ? 0 : 1;

        using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryPath, writable: true) 
        ?? Registry.CurrentUser.CreateSubKey(RegistryPath)) {
            key.SetValue(
                "AppsUseLightTheme",
                value,
                RegistryValueKind.DWord
            );
        
            key.SetValue(
                "AppsUsesLightTheme",
                value,
                RegistryValueKind.DWord
            );

            key.SetValue(
                "SystemUseLightTheme",
                value,
                RegistryValueKind.DWord
            );
        
            key.SetValue(
                "SystemUsesLightTheme",
                value,
                RegistryValueKind.DWord
            );
        }
    }
}