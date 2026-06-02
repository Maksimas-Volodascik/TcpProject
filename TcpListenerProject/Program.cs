using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TcpListenerProject.TeltonikaDataParser;
using TcpListenerProject.TeltonikaDataParser.Decoder;
using TcpListenerProject.TeltonikaDataParser.Header;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TcpListenerProject
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder();

            builder.Services.AddNpgsql<DataContext>(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);

            builder.Services.AddScoped<IProcessDataService, ProcessDataService>();
            builder.Services.AddScoped<IPacketParser, PacketParser>();
            builder.Services.AddScoped<IDecoderFactory, DecoderFactory>();
            builder.Services.AddScoped<ITeltonikaParser, TeltonikaParser>();
            builder.Services.AddScoped<IDecoder, Codec8Parser>();
            builder.Services.AddScoped<IDecoder, Codec8EParser>();

            builder.Services.AddSingleton<TcpServer>();

            var host = builder.Build();

            var server = host.Services.GetRequiredService<TcpServer>();

            await server.ServerListener();
        }
    }
}
