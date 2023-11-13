using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using WeatherSimulator.Server.Models;
using WeatherSimulator.Server.Storages.Abstractions;

namespace WeatherSimulator.Server.Storages
{
    public class LastMeasureStore : ILastMeasureStore
    {
        private readonly object _locker = new();
        private readonly ConcurrentDictionary<Guid, SensorMeasure> _itemsDict = new();

        public void AddMeasure(SensorMeasure measure)
        {
            _itemsDict.AddOrUpdate(measure.SensorId,
                measure,
                (id, _) => measure);
        }

        public SensorMeasure? GetMeasure(Guid sensorId)
        {
            return _itemsDict.GetValueOrDefault(sensorId);
        }
    }
}
