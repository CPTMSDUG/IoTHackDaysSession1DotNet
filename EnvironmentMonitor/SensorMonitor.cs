using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EnvironmentMonitor.Models;
using Iot.Device.DHTxx;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using UnitsNet;

namespace EnvironmentMonitor
{
    public class SensorMonitor
    {
        private readonly DeviceClient _deviceClient;

        private double _temperature;
        private double _humidity;

        Dht11 dht11 = new Dht11(23);
         public SensorMonitor(DeviceClient deviceClient)
        {
            _deviceClient = deviceClient ?? throw new ArgumentNullException(nameof(deviceClient));
        }

        public async Task PerformOperationsAsync(CancellationToken cancellationToken)
        {

            while (!cancellationToken.IsCancellationRequested)
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
                _temperature = lastTemperature;
                _humidity = lastHumidity;
                Console.WriteLine($"Humidity: {lastHumidity}%");
                Console.WriteLine($"Temperature: {lastTemperature}C");

                await SendTemperatureTelemetryAsync(cancellationToken);
            }

                await Task.Delay(5 * 1000, cancellationToken);
            }
        }

 private async Task SendTemperatureTelemetryAsync(CancellationToken cancellationToken)
        {
            var telemetry = new SensorData();
            telemetry.Temperature = _temperature;
            telemetry.Humidity = _humidity;
            var telemetryPayload = JsonSerializer.Serialize<SensorData>(telemetry); 

            using var message = new Message(Encoding.UTF8.GetBytes(telemetryPayload))
            {
                ContentEncoding = "utf-8",
                ContentType = "application/json",
            };

            await _deviceClient.SendEventAsync(message, cancellationToken);
            Console.WriteLine($"Telemetry: Sent - {telemetryPayload}");
        }
    }
}