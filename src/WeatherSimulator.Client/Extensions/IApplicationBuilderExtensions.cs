using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace WeatherSimulator.Client.Extensions
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder appBuilder)
        {
            return appBuilder.UseExceptionHandler(app =>
            {
                app.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var errorHandler = context.Features.Get<IExceptionHandlerFeature>();
                    if (errorHandler?.Error is Grpc.Core.RpcException error)
                    {
                        if (error.StatusCode == Grpc.Core.StatusCode.NotFound)
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                        }
                        var message = error.Status.Detail;
                        await context.Response.WriteAsJsonAsync(new { message });
                    }
                });
            });
        }
    }
}
