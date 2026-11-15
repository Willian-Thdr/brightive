using System.Runtime.InteropServices;
using Windows.AI.MachineLearning;
using Windows.UI.Notifications;

class main
{
    [DllImport("kernel32.dll")]
    private static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    private const int SW_HIDE = 0;

    public static void Main(string[] args)
    {
        var handle = GetConsoleWindow();
        ShowWindow(handle, SW_HIDE);
        create_config();

        CameraSensor cam_sen = new CameraSensor();
        cam_sen.use_camera();
        Console.ReadKey();
    }

    private static void create_config()
    {
        string way = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string save_config = Path.Combine(way, "Brightive", "config");

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

        if (!Directory.Exists(save_config))
        {
            try
            {
                string filename = Path.Combine(save_config, "config.cfg");
                string content = """
                min_bright:120
                max_bright:160
                """;
                
                File.WriteAllText(filename, content);
            } 
            catch(Exception e)
            {
                Console.WriteLine($"Error: {e}");
            }
        }
    }
}
