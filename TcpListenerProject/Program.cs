using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System.Configuration;
using TcpListenerProject.TeltonikaDataParser;
using TcpListenerProject.TeltonikaDataParser.Decoder;
using TcpListenerProject.TeltonikaDataParser.Interfaces;
using TcpListenerProject.TeltonikaDataParser.Protocol;

namespace TcpListenerProject
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder();

            //builder.Services.AddNpgsql<DataContext>(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            builder.Services.AddNpgsql<DataContext>(Environment.GetEnvironmentVariable("DB_CONNECTION") 
                ?? System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);

            builder.Services.AddScoped<IProcessDataService, ProcessDataService>();
            builder.Services.AddScoped<IPacketParser, PacketParser>();
            builder.Services.AddScoped<IDecoderFactory, DecoderFactory>();
            builder.Services.AddScoped<ITeltonikaParser, TeltonikaParser>();
            builder.Services.AddScoped<IDecoder, Codec8Parser>();
            builder.Services.AddScoped<IDecoder, Codec8EParser>();

            builder.Services.AddSingleton<TcpServer>();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext() //Save unique correlation ID per connection.
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning) // EF filter
                .WriteTo.Console(outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level:u3}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            builder.Logging.ClearProviders();
            builder.Services.AddSerilog();

            var host = builder.Build();

            var server = host.Services.GetRequiredService<TcpServer>();

            await server.ServerListener();
        }
    }
}
