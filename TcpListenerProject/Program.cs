using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
            builder.Services.AddSingleton<TcpServer>();

            var host = builder.Build();

            var server = host.Services.GetRequiredService<TcpServer>();

            await server.ServerListener();
        }
    }
}
