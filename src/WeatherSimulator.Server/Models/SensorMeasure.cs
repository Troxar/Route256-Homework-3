using System;
using WeatherSimulator.Proto;

namespace WeatherSimulator.Server.Models;

public class SensorMeasure
{
    public SensorMeasure(Guid sensorId,
        double temperature,
        int humidity,
        int co2,
        Enums.SensorLocationType locationType)
    {
        SensorId = sensorId;
        Temperature = temperature;
        Humidity = humidity;
        CO2 = co2;
        LocationType = locationType;
        LastUpdate = DateTime.Now;
    }

    /// <summary>
    /// Идентификатор сенсора
    /// </summary>
    public Guid SensorId { get; private set; }

    /// <summary>
    /// Температура
    /// </summary>
    public double Temperature { get; private set; }

    /// <summary>
    /// Влажность
    /// </summary>
    public int Humidity { get; private set; }

    /// <summary>
    /// Показатель CO2
    /// </summary>
    public int CO2 { get; private set; }

    public Enums.SensorLocationType LocationType { get; private set; }

    /// <summary>
    /// Время последнего обновления сенсора
    /// </summary>
    public DateTime LastUpdate { get; private set; }

    public SensorData ToSensorData()
    {
        return new SensorData
        {
            SensorId = SensorId.ToString(),
            Temperature = Temperature,
            Humidity = Humidity,
            Co2 = CO2,
            LocationType = (Proto.SensorLocationType)LocationType
        };
    }
}
