using WeatherSimulator.Proto;

namespace WeatherSimulator.Client.Services.Abstractions
{
    public interface IMeasureGrpcServiceClient
    {
        Task<SensorData> GetLastMeasure(string sensorId);
    }
}
