using Windows.Devices.Sensors;

class Sensor
{
    public static void Main()
    {
        CameraSensor cam_sen = new CameraSensor();

        var sensor = LightSensor.GetDefault();
        
        if (sensor is not null)
        {
            Console.WriteLine("Sensor encontrado!");
        
            var reading = sensor.GetCurrentReading();
        
            Console.WriteLine(
                $"Luminosidade: {reading.IlluminanceInLux} lux"
            );
        }
        else
        {
            Console.WriteLine("Nenhum sensor encontrado.");
            cam_sen.use_camera();
        }
        
        Console.ReadKey();
    }
}
