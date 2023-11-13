using Microsoft.AspNetCore.Mvc;
using WeatherSimulator.Client.Services.Abstractions;

namespace WeatherSimulator.Client.Controllers
{
    [Route("/sensor")]
    public class SensorController : Controller
    {
        private readonly IMeasureGrpcServiceClient _client;

        public SensorController(IMeasureGrpcServiceClient client)
        {
            _client = client;
        }

        [Route("{id:guid}")]
        public async Task<IActionResult> GetLastMeasure(string id)
        {
            var response = await _client.GetLastMeasure(id);
            return Json(response);
        }
    }
}
