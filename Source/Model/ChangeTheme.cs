using System;
using System.Runtime.InteropServices;
using Microsoft.Win32;

public class ChangeTheme
{
    private const string RegistryPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";

    [DllImport("user32.dll", EntryPoint = "SendMessageTimeoutW", CharSet = CharSet.Unicode, ExactSpelling = true)]
    private static extern IntPtr SendMessageTimeout(
        IntPtr hWnd,
        uint Msg,
        IntPtr wParam,
        string lParam,
        uint fuFlags,
        uint uTimeout,
        out IntPtr result
    );

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
                "SystemUsesLightTheme",
                value,
                RegistryValueKind.DWord
            );
        }

        RefreshTheme();
    }

    private static void RefreshTheme()
    {
        SendMessageTimeout(
            new nint(-1),
            0x001A,
            IntPtr.Zero,
            "ImmersiveColorSet",
            0x0002,
            5000,
            out _
        );
    }
}