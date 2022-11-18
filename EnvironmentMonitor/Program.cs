using EnvironmentMonitor;
using Iot.Device.DHTxx;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Provisioning.Client;
using Microsoft.Azure.Devices.Provisioning.Client.Transport;
using Microsoft.Azure.Devices.Shared;
using UnitsNet;

namespace Welcome;
class Program
{
    private const string ModelId = "<MODELID>";

    private const string DeviceId = "<DEVICEID>";
    private const string DeviceSymmetricKey = "<DEVICEKEY>";
    private const string DpsEndpoint = "global.azure-devices-provisioning.net";
    private const string DpsIdScope = "<SCOPEID>";
    
    static async Task Main(string[] args)
    {
            var cts = new CancellationTokenSource();

            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                cts.Cancel();
            };


        try
        {
            DeviceClient deviceClient = await InitializeDeviceClient(cts.Token);
            
            var sample = new SensorMonitor(deviceClient);
            await sample.PerformOperationsAsync(cts.Token);

            await deviceClient.CloseAsync(CancellationToken.None);
        }
        catch (OperationCanceledException) { } // User canceled operation

    }

    private static async Task<DeviceClient> InitializeDeviceClient(CancellationToken cancellationToken)
    {
 
                    DeviceRegistrationResult dpsRegistrationResult = await ProvisionDeviceAsync(cancellationToken);
                    var authMethod = new DeviceAuthenticationWithRegistrySymmetricKey(dpsRegistrationResult.DeviceId, DeviceSymmetricKey);
            
                DeviceClient deviceClient = DeviceClient.Create(dpsRegistrationResult.AssignedHub, authMethod, TransportType.Mqtt);

        return deviceClient;
    }

     private static async Task<DeviceRegistrationResult> ProvisionDeviceAsync(CancellationToken cancellationToken)
        {
            using SecurityProvider symmetricKeyProvider = new SecurityProviderSymmetricKey(DeviceId, DeviceSymmetricKey, null);
            using ProvisioningTransportHandler mqttTransportHandler = new ProvisioningTransportHandlerMqtt();
            ProvisioningDeviceClient pdc = ProvisioningDeviceClient.Create(DpsEndpoint, DpsIdScope,
                symmetricKeyProvider, mqttTransportHandler);

            var pnpPayload = new ProvisioningRegistrationAdditionalData
            {
                JsonData = $"{{ \"modelId\": \"{ModelId}\" }}",
            };
            return await pdc.RegisterAsync(pnpPayload, cancellationToken);
        }
}