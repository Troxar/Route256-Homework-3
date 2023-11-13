using Grpc.Net.Client;
using WeatherSimulator.Client.Services.Abstractions;
using WeatherSimulator.Proto;

namespace WeatherSimulator.Client.Services
{
    public class MeasureGrpcServiceClient : IMeasureGrpcServiceClient
    {
        private readonly WeatherSimulatorService.WeatherSimulatorServiceClient _client;

        public MeasureGrpcServiceClient()
        {
            var channel = GrpcChannel.ForAddress("http://localhost:5082");
            _client = new WeatherSimulatorService.WeatherSimulatorServiceClient(channel);
        }

        public async Task<SensorData> GetLastMeasure(string sensorId)
        {
            var request = new GetLastMeasureRequest { SensorId = sensorId };
            return await _client.GetLastMeasureAsync(request);
        }
    }
}
