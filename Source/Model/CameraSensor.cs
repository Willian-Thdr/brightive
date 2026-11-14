using OpenCvSharp;
using System.Diagnostics;

public class CameraSensor
{
    public void use_camera()
    {
        using var camera = new VideoCapture(0, VideoCaptureAPIs.MSMF);
        
        if (!camera.IsOpened())
        {
            Console.WriteLine("Open camera it's not possible.");
            return;
        }
        
        Console.WriteLine("Camera initializing.");
        
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

            if (brightness > 125)
            {
                ChangeTheme.change(false);
            } 
            else if (brightness < 120)
            {
                ChangeTheme.change(true);    
            }

            Thread.Sleep(1000);
        }
    }
}