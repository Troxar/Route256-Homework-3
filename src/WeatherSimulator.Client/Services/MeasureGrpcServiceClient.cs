using WeatherSimulator.Client.Services.Abstractions;
using WeatherSimulator.Proto;

namespace WeatherSimulator.Client.Services
{
    public class MeasureGrpcServiceClient : IMeasureGrpcServiceClient
    {
        private readonly WeatherSimulatorService.WeatherSimulatorServiceClient _client;

        public MeasureGrpcServiceClient(WeatherSimulatorService.WeatherSimulatorServiceClient client)
        {
            _client = client;
        }

        public async Task<SensorData> GetLastMeasure(string sensorId)
        {
            var request = new GetLastMeasureRequest { SensorId = sensorId };
            return await _client.GetLastMeasureAsync(request);
        }
    }
}
