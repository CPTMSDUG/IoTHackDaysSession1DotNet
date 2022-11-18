using Iot.Device.DHTxx;
using UnitsNet;

namespace Welcome;
class Program
{
   
    static void Main(string[] args)
    {
        Dht11 dht11 = new Dht11(23);
  
        while (true)
        {

            double lastTemperature = 0.0;
            double lastHumidity = 0.0;

            if (dht11.TryReadHumidity(out RelativeHumidity humidity))
            {
                lastHumidity = humidity.Percent;

            }

            if (dht11.TryReadTemperature(out Temperature temperature))
            {
                lastTemperature = temperature.DegreesCelsius;
            }

            if (dht11.IsLastReadSuccessful)
            {
                Console.WriteLine($"Humidity: {lastHumidity}%");
                Console.WriteLine($"Temperature: {lastTemperature}C");
            }
            Thread.Sleep(5000);
        }
    }
}