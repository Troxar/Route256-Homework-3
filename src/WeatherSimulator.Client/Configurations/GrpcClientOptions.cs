using System.ComponentModel.DataAnnotations;

namespace WeatherSimulator.Client.Configurations;

public class GrpcClientOptions
{
    [Required]
    public string Host { get; set; }

    [Required]
    public int Port { get; set; }

    public bool UseHttps { get; set; }
}
