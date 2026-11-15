using OpenCvSharp;
using System.Diagnostics;
using System.Runtime.InteropServices;

public class CameraSensor
{
    public void use_camera()
    {
        List<int> list = get_config();

        using var camera = new VideoCapture(0, VideoCaptureAPIs.MSMF);
        
        if (!camera.IsOpened())
        {
            Console.WriteLine("Open camera it's not possible.");
            return;
        }
        
        using var frame = new Mat();
        using var gray = new Mat();
        
        var timer = Stopwatch.StartNew();
        
        while(true)
        {
            if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape)
                break;
        
            if (!camera.Read(frame) || frame.Empty())
                continue;
        
            Cv2.CvtColor(
                frame,
                gray,
                ColorConversionCodes.BGR2GRAY
            );
        
            double brightness = Cv2.Mean(gray).Val0;

            if (brightness > list[1])
            {
                ChangeTheme.change(false);
            } 
            else if (brightness < list[0])
            {
                ChangeTheme.change(true);
            }

            Thread.Sleep(100);
        }
    }

    private static List<int> get_config()
    {
        string way = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string config_way = Path.Combine(way, "Brightive", "config");
        string file = Path.Combine(config_way, "config.cfg");

        string[] content = File.ReadAllLines(file);

        List<int> list = new List<int>();

        foreach(string lines in content)
        {
            string line = lines.Trim().Trim(';').Trim(']');

            if (line.StartsWith("min_bright"))
            {
                int min_bright = int.Parse(lines.Substring(11).Trim());
                list.Add(min_bright);
            } 
            else if (line.StartsWith("max_bright"))
            {
                int max_bright = int.Parse(lines.Substring(11).Trim());
                list.Add(max_bright);
            }
        }

        return list;
    }
}