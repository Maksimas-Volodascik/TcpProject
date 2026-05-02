using System.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace TcpListenerProject
{
    public class TcpServer
    {
        private readonly IServiceScopeFactory _scopeFactory;
        public TcpServer(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task ServerListener()
        {
            TcpListener server;
            Int32 port;
            IPAddress ipAddress;

            if (!IPAddress.TryParse(ConfigurationManager.AppSettings["ipAddress"], out ipAddress))
            {
                ipAddress = IPAddress.Any;
            }

            if (!Int32.TryParse(ConfigurationManager.AppSettings["port"], out port))
            {
                port = 13000;
            }

            server = new TcpListener(ipAddress, port);

            Console.Write("Starting server...\n");

            server.Start();

            Console.Write("Waiting for a connection... \n");
            try
            {
                while (true)
                {
                    TcpClient client = await server.AcceptTcpClientAsync();

                    _ = HandleClientAsync(client);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("\n Socket exception: {0}", e.Message);
            }
            finally
            {
                server.Stop();
            }

            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }

        public async Task HandleClientAsync(TcpClient client)
        {
            using var scope = _scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IProcessDataService>();
            
            try
            {
                var stream = client.GetStream();
                using var reader = new StreamReader(stream);
                byte[] bytes = new Byte[2048];

                int imeiBytes = await stream.ReadAsync(bytes);
                Console.WriteLine(imeiBytes);
                if (imeiBytes == 0) // client disconnect
                {
                    Console.WriteLine("\nDisconnected");
                    return;
                }

                string imeiString = System.Text.Encoding.UTF8.GetString(bytes, 0, imeiBytes);
                Console.WriteLine("\n{0} Connecting...", imeiString);

                string imeiAck = "01";
                byte[] imeiResponse = System.Text.Encoding.UTF8.GetBytes(imeiAck);
                await stream.WriteAsync(imeiResponse, 0, imeiResponse.Length);
                Console.WriteLine("{0} Connection established", imeiString);

                while (true)
                {
                    int bytesRead = await stream.ReadAsync(bytes);

                    if (bytesRead == 0) // client disconnect
                    {
                        Console.WriteLine("\nDisconnected");
                        break;
                    }

                    string data = System.Text.Encoding.UTF8.GetString(bytes, 0, bytesRead);
                    Console.WriteLine("\n Received {0}", data);

                    // send back acknowledgement
                    string responseMsg = "01";
                    byte[] resp = System.Text.Encoding.UTF8.GetBytes(responseMsg);
                    await stream.WriteAsync(resp, 0, resp.Length);
                    Console.WriteLine("\n Sent: {0} \n", responseMsg);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Socket error: {0}", e.Message);
            }
            finally
            {
                client.Close();
            }
        }
    }
}
