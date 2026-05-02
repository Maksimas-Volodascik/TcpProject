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
            var processDataService = scope.ServiceProvider.GetRequiredService<IProcessDataService>();
            
            try
            {
                var networkStream = client.GetStream();
                byte[] buffer = new Byte[2048];

                int imeiByteCount = await networkStream.ReadAsync(buffer);

                if (imeiByteCount == 0) // client disconnect
                {
                    Console.WriteLine("\nDisconnected");
                    return;
                }

                string imei = System.Text.Encoding.UTF8.GetString(buffer, 0, imeiByteCount);

                Console.WriteLine("\n{0} Connecting...", imei);

                try
                {
                    await processDataService.GetDeviceByImeiAsync(imei);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }

                string imeiAcknowledgement = "01";
                byte[] imeiResponse = System.Text.Encoding.UTF8.GetBytes(imeiAcknowledgement);
                await networkStream.WriteAsync(imeiResponse, 0, imeiResponse.Length);

                Console.WriteLine("{0} Connection established", imei);

                while (true)
                {
                    int bytesRead = await networkStream.ReadAsync(buffer);

                    if (bytesRead == 0) // client disconnect
                    {
                        Console.WriteLine("\nDisconnected");
                        break;
                    }

                    string data = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    await processDataService.SaveRawRecordAsync(imei, data);

                    Console.WriteLine("\n Received {0}", data);
                    
                    // send back acknowledgement
                    string responseMsg = "01";
                    byte[] acknowledgementBytes = System.Text.Encoding.UTF8.GetBytes(responseMsg);
                    await networkStream.WriteAsync(acknowledgementBytes, 0, acknowledgementBytes.Length);
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
