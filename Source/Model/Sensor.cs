using Microsoft.Win32;
using System;

class Sensor
{
    public static void Main()
    {
        CameraSensor cam_sen = new CameraSensor();

        cam_sen.use_camera();
        
        Console.ReadKey();
    }

    private const string RegistryPath = @"Software\Microsoft\Windows\CurrentVersion\Run";

    public static void Enable()
    {
        using RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryPath, true) 
        ?? Registry.CurrentUser.CreateSubKey(RegistryPath);

        string exePath = Environment.ProcessPath
            ?? throw new InvalidOperationException("Executable path unavailable.");

        key.SetValue("Brightive", $"\"{exePath}\"");
    }

    public static void Disable()
    {
        using RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryPath, true);

        string exePath = Environment.ProcessPath;

        key.SetValue("Brightive", false);
    }
}
