using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TcpListenerProject
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var service = new ServiceCollection();

            service.AddDbContext<DataContext>(opt => opt.UseNpgsql(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString));

            service.AddScoped<ProcessDataService>();

            service.AddSingleton<TcpServer>();

            var provider = service.BuildServiceProvider();

            var server = provider.GetRequiredService<TcpServer>();
            await server.ServerListener();
        }
    }
}
