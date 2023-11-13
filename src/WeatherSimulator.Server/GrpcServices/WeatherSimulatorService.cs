using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WeatherSimulator.Proto;
using WeatherSimulator.Server.Models;
using WeatherSimulator.Server.Services.Abstractions;
using static WeatherSimulator.Proto.WeatherSimulatorService;

namespace WeatherSimulator.Server.GrpcServices;

public class WeatherSimulatorService : WeatherSimulatorServiceBase
{
    private readonly IMeasureService _measureService;
    private readonly ILogger<WeatherSimulatorService> _logger;

    public WeatherSimulatorService(
        IMeasureService measureService,
        ILogger<WeatherSimulatorService> logger)
    {
        _measureService = measureService;
        _logger = logger;
    }

    public override async Task GetSensorsStream(IAsyncStreamReader<ToServerMessage> requestStream, IServerStreamWriter<SensorData> responseStream, ServerCallContext context)
    {
        await ProceedMessage(requestStream, responseStream, context.CancellationToken);
    }

    private async Task ProceedMessage(IAsyncStreamReader<ToServerMessage> requestStream,
        IServerStreamWriter<SensorData> responseStream,
        CancellationToken cancellationToken)
    {
        ConcurrentDictionary<Guid, Guid> sensorSubscriptionIds = new();
        while (await requestStream.MoveNext() && !cancellationToken.IsCancellationRequested)
        {
            var current = requestStream.Current;
            if (current.SubscribeSensorsIds is not null)
                Subscribe(responseStream, sensorSubscriptionIds, cancellationToken, current);

            if (current.UnsubscribeSensorsIds is not null)
                Unsubscribe(sensorSubscriptionIds, current);
        }
    }

    private void Subscribe(IServerStreamWriter<SensorData> responseStream, ConcurrentDictionary<Guid, Guid> sensorSubscriptionIds,
        CancellationToken cancellationToken, ToServerMessage current)
    {
        foreach (var id in current.SubscribeSensorsIds)
        {
            if (Guid.TryParse(id, out var tempId) && !sensorSubscriptionIds.TryGetValue(tempId, out Guid _))
            {
                var containsSub = sensorSubscriptionIds.TryGetValue(tempId, out Guid subscriptionId);
                if (!containsSub)
                {
                    sensorSubscriptionIds[tempId] = _measureService.SubscribeToMeasures(tempId,
                        async measure => await OnNewMeasure(responseStream, measure, cancellationToken), cancellationToken);
                    _logger.LogDebug("Subscribed!");
                }
            }
        }
    }

    private void Unsubscribe(ConcurrentDictionary<Guid, Guid> sensorSubscriptionIds, ToServerMessage current)
    {
        foreach (var id in current.UnsubscribeSensorsIds)
        {
            if (!Guid.TryParse(id, out var tempId) ||
                !sensorSubscriptionIds.TryGetValue(tempId, out Guid subscriptionId))
                continue;

            _measureService.UnsubscribeFromMeasures(tempId, subscriptionId);
            sensorSubscriptionIds.Remove(tempId, out _);
            _logger.LogDebug("Unsubscribed!");
        }
    }

    private static async Task OnNewMeasure(IAsyncStreamWriter<SensorData> responseStream, SensorMeasure measure, CancellationToken cancellationToken)
    {
        await responseStream.WriteAsync(new SensorData()
        {
            SensorId = measure.SensorId.ToString(),
            Temperature = measure.Temperature,
            Humidity = measure.Humidity,
            Co2 = measure.CO2,
            LocationType = (Proto.SensorLocationType)measure.LocationType
        }, cancellationToken);
    }

    public override Task<SensorData> GetLastMeasure(GetLastMeasureRequest request, ServerCallContext context)
    {
        var sensor = _measureService
            .GetAvailableSensors()
            .FirstOrDefault(sensor => sensor.Id.ToString() == request.SensorId);

        if (sensor is null)
        {
            _logger.LogError("Sensor id not found: {sensorId}", request.SensorId);
            throw new RpcException(new Status(StatusCode.NotFound, $"Sensor id not found: {request.SensorId}"));
        }

        var randGen = new Random();

        var sensorData = new SensorData
        {
            SensorId = sensor.Id.ToString(),
            Temperature = randGen.Next(200, 320) / 10,
            Humidity = randGen.Next(40, 60),
            Co2 = sensor.LocationType == Models.Enums.SensorLocationType.External
                ? randGen.Next(350, 360)
                : randGen.Next(400, 600)
        };

        return Task.FromResult(sensorData);
    }
}