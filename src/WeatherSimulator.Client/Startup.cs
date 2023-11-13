using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using WeatherSimulator.Client.Configurations;
using WeatherSimulator.Client.Extensions;
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
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Weather simulator gRPC API",
                Version = "v1"
            });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            });
        }
        else
        {
            app.UseCustomExceptionHandler();
        }

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
