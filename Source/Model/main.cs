using System.Runtime.InteropServices;
using System.Timers;
using Microsoft.Win32;
using Timer = System.Timers.Timer;

public class main
{
    // DLL importadas

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern bool SystemParametersInfo(
        int uAction,
        int uParam,
        string lpcParam,
        int fuWinIni
    );

    private const int SPI_SETDESKTOPWALLPAPER = 20;
    private const int SPIF_UPDATEINiFILE = 0x01;
    private const int SPIF_SENDCHANGE = 0x02;

    public static void set_wallpaper(string path_light)
    {
        bool result = SystemParametersInfo(SPI_SETDESKTOPWALLPAPER, 0, path_light, SPIF_UPDATEINiFILE | SPIF_SENDCHANGE);
    }

    // Métodos e classes

    private const int SW_HIDE = 0;

    public enum win_theme
    {
        Light, 
        Dark,
        Unknown,
    }

    public static void Main(string[] args)
    {
        var handle = GetConsoleWindow();
        ShowWindow(handle, SW_HIDE);

        create_config();

        List<int> milliseconds = CameraSensor.Get_config();

        if (milliseconds[2] != 0)
        {
            Timer aTimer = new Timer(milliseconds[2]);
            Timer eTimer = new Timer(100);
            
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
    
    
            eTimer.Elapsed += alterWallpaperEvent;
            eTimer.AutoReset = true;
            eTimer.Enabled = true;
        }
        
        CameraSensor cam_sen = new CameraSensor();
        cam_sen.use_camera();
        
    }

    private static void create_config()
    {
        string way = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string save_config = Path.Combine(way, "Brightive", "config");
        string file_config = Path.Combine(save_config, "config.cfg");
        string file_config_wall_light = Path.Combine(save_config, "wallpaper_light.cfg");
        string file_config_wall_dark = Path.Combine(save_config, "wallpaper_dark.cfg");

        if (!Directory.Exists(save_config))
        {
            try
            {
                Directory.CreateDirectory(save_config);
            } catch(Exception e)
            {
                Console.WriteLine($"Error: {e}");
            }
        }

        if (!File.Exists(file_config))
        {
            try
            {
                string content = """
                !! NOTE: 1 second is equals to 1000 milliseconds !!

                min_bright: 120
                max_bright: 160
                wallpaper_time (milliseconds): 60000
                """;
                
                File.WriteAllText(file_config, content);
            } 
            catch(Exception e)
            {
                Console.WriteLine($"Error: {e}");
            }
        }

        if (!File.Exists(file_config_wall_light))
        {
            try
            {
                string content = """
                light: [
                    C:\Windows\Web\Wallpaper\Windows\img0.jpg, 
                    C:\Windows\Web\Wallpaper\ThemeD\img32.jpg,  
                    C:\Windows\Web\Wallpaper\ThemeC\img28.jpg, 
                    C:\Windows\Web\Wallpaper\ThemeC\img29.jpg,  
                    C:\Windows\Web\Wallpaper\ThemeC\img30.jpg, 
                    C:\Windows\Web\Wallpaper\ThemeC\img31.jpg, 
                    C:\Windows\Web\Wallpaper\ThemeD\img33.jpg, 
                    C:\Windows\Web\Wallpaper\ThemeD\img34.jpg,  
                    C:\Windows\Web\Wallpaper\ThemeD\img35.jpg
                ]
                """;
                
                File.WriteAllText(file_config_wall_light, content);
            } 
            catch(Exception e)
            {
                Console.WriteLine($"Error: {e}");
            }
        }

        if (!File.Exists(file_config_wall_dark))
        {
            try
            {
                string content = """
                dark: [
                    C:\Windows\Web\Wallpaper\Windows\img19.jpg, 
                    C:\Windows\Web\Wallpaper\ThemeB\img24.jpg,
                    C:\Windows\Web\Wallpaper\ThemeB\img25.jpg, 
                    C:\Windows\Web\Wallpaper\ThemeB\img26.jpg,
                    C:\Windows\Web\Wallpaper\ThemeB\img27.jpg, 
                    C:\Windows\Web\Wallpaper\ThemeA\img20.jpg,
                    C:\Windows\Web\Wallpaper\ThemeA\img21.jpg, 
                    C:\Windows\Web\Wallpaper\ThemeA\img22.jpg, 
                    C:\Windows\Web\Wallpaper\ThemeA\img23.jpg
                ] 
                """;
                
                File.WriteAllText(file_config_wall_dark, content);
            } 
            catch(Exception e)
            {
                Console.WriteLine($"Error: {e}");
            }
        }
    }

    public static win_theme themeSelector()
    {
        const string keyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";

        using RegistryKey? key = Registry.CurrentUser.OpenSubKey(keyPath);

        if (key == null)
            return win_theme.Unknown;

        object? value = key.GetValue("SystemUsesLightTheme");

        return value switch
        {
            1 => win_theme.Light,
            0 => win_theme.Dark,
            _ => win_theme.Unknown
        };
    }

    private static void OnTimedEvent(Object? source, ElapsedEventArgs args)
    {
        win_theme value = themeSelector();
        List<string> wallpaper_light_list = read_wallpaper_config("light");
        List<string> wallpaper_dark_list = read_wallpaper_config("dark");

        if (value == win_theme.Dark)
        {
            Random rand = new Random();
            int num = wallpaper_dark_list.Count;

            set_wallpaper(wallpaper_dark_list[rand.Next(num)]);
        }
        else if (value == win_theme.Light)
        {
            Random rand = new Random();
            int num = wallpaper_light_list.Count;

            set_wallpaper(wallpaper_light_list[rand.Next(num)]);
        }
        else
        {
            Console.WriteLine("Error");
            return;
        }
    }

    private static win_theme last_theme = win_theme.Unknown;

    private static void alterWallpaperEvent(Object? source, ElapsedEventArgs args)
    {
        win_theme val = themeSelector();        
        List<string> wallpaper_light_list = read_wallpaper_config("light");
        List<string> wallpaper_dark_list = read_wallpaper_config("dark");
        
        if (last_theme != val)
        {
            switch (val)
            {
                case win_theme.Light:
                    Random rand = new Random();
                    int num = wallpaper_light_list.Count;

                    set_wallpaper(wallpaper_light_list[rand.Next(num)]);
                break;

                case win_theme.Dark:
                    rand = new Random();
                    num = wallpaper_dark_list.Count;

                    set_wallpaper(wallpaper_dark_list[rand.Next(num)]);
                break;
            }
        }

        last_theme = themeSelector();
    }


    public static List<string> read_wallpaper_config(string condition)
    {
        string main_way = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string path_light = Path.Combine(main_way, "Brightive", "config", "wallpaper_light.cfg");
        string path_dark = Path.Combine(main_way, "Brightive", "config", "wallpaper_dark.cfg");
        string[] lines_light = File.ReadAllLines(path_light);
        string[] lines_dark = File.ReadAllLines(path_dark);

        List<string> wallpapers = new List<string>();


        switch (condition)
        {
            case "light":
                foreach(string line in lines_light)
                {
                    string new_line = line.Trim(' ', '.', ',', '!', '?', '"', '@', '[', ']', ':');
                    string clean = new_line.Replace("dark", "").Replace("light", "");

                    if (!clean.StartsWith("dark") && !clean.StartsWith("light") && !string.IsNullOrWhiteSpace(clean))
                    {
                        wallpapers.Add(clean.Trim());   
                    }

                    if (new_line.StartsWith("dark"))
                    {
                        new_line.Equals("dark", StringComparison.OrdinalIgnoreCase);
                        break;
                    }
                }
            break;

            case "dark":
                foreach(string line in lines_dark)
                {
                    string new_line = line.Trim(' ', '.', ',', '!', '?', '"', '@', '[', ']', ':');
                    string clean = new_line.Replace("dark", "").Replace("light", "");

                    if (!clean.StartsWith("dark") && !clean.StartsWith("light") && !string.IsNullOrWhiteSpace(clean))
                    {
                        wallpapers.Add(clean.Trim());   
                    }

                    if (new_line.StartsWith("light"))
                    {
                        new_line.Equals("light", StringComparison.OrdinalIgnoreCase);
                        break;
                    }
                }
            break;
        }

        return wallpapers;
    }
}
