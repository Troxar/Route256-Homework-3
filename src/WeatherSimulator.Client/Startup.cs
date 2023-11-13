using Microsoft.Extensions.Options;
using WeatherSimulator.Client.Configurations;
using WeatherSimulator.Client.Services;
using WeatherSimulator.Client.Services.Abstractions;
using WeatherSimulator.Proto;

namespace WeatherSimulator.Client;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.Configure<GrpcClientOptions>(_configuration.GetSection("GrpcClient"));
        services.AddGrpcClient<WeatherSimulatorService.WeatherSimulatorServiceClient>((provider, options) =>
        {
            var grpcOptions = provider.GetRequiredService<IOptions<GrpcClientOptions>>().Value;
            options.Address = new Uri($"{(grpcOptions.UseHttps ? "https" : "http")}" +
                $"://{grpcOptions.Host}:{grpcOptions.Port}");
        });
        services.AddLogging();
        services.AddControllers();
        services.AddSingleton<IMeasureGrpcServiceClient, MeasureGrpcServiceClient>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
