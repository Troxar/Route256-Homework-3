using System;
using WeatherSimulator.Server.Models;

namespace WeatherSimulator.Server.Storages.Abstractions;

public interface ILastMeasureStore
{
    SensorMeasure? GetMeasure(Guid sensorId);
    void AddMeasure(SensorMeasure measure);
}
