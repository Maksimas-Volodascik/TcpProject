using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TcpListenerProject.TeltonikaDataParser;
using TcpListenerProject.TeltonikaDataParser.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TcpListenerProject
{
    public class TcpServer
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ITeltonikaParser _teltonikaParser;
        private readonly ConcurrentDictionary<string, int> _connectionCounts = new();
        private readonly ConcurrentDictionary<string, (int Count, DateTime WindowStart)> _messageLimit = new();
        private readonly TcpListener _listener;
        private const int MaxConnectionsPerIp = 3;
        private const int MaxMessagesPerDevice = 3;

        public TcpServer(IServiceScopeFactory scopeFactory, ITeltonikaParser teltonikaParser)
        {
            _scopeFactory = scopeFactory;
            _teltonikaParser = teltonikaParser;

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

            _listener = new TcpListener(ipAddress, port);
        }

        public async Task ServerListener()
        {
            Console.Write("Starting server...\n");
            _listener.Start();
            Console.Write("Waiting for a connection... \n");
            try
            {
                while (true)
                {
                    TcpClient client = await _listener.AcceptTcpClientAsync();

                    string ip = ((IPEndPoint)client.Client.RemoteEndPoint!).Address.ToString();
                        
                    int current = _connectionCounts.AddOrUpdate(ip, 1, (_, count) => count + 1);

                    if (current > MaxConnectionsPerIp)
                    {
                        Console.WriteLine($"[REJECTED] {ip} exceeded connection limit ({current}/{MaxConnectionsPerIp})");
                        _connectionCounts.AddOrUpdate(ip, 0, (_, count) => count - 1);
                        client.Close();
                        continue;
                    }

                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await HandleClientAsync(client, ip);
                        }
                        finally
                        {
                            _connectionCounts.AddOrUpdate(ip, 0, (_, count) => Math.Max(0, count - 1));
                        }
                    });
                        
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("\n Socket exception: {0}", e.Message);
            }

            await Task.Delay(TimeSpan.FromSeconds(5));

        }

        private bool IsRateLimited(string ip)
        {
            var now = DateTime.UtcNow;

            var updated = _messageLimit.AddOrUpdate(
                ip,
                _ => (1, now),
                (_, existing) =>
                {
                    if((now - existing.WindowStart).TotalSeconds >= 1)
                    {
                        return (1, now);
                    }

                    return (existing.Count + 1, existing.WindowStart);
                });

            if (updated.Count > MaxMessagesPerDevice)
            {
                Console.WriteLine($"[RATE LIMITED] {ip} sent {updated.Count} msgs in current window");
                return true;
            }
            return false;
        }
        
        private async Task HandleClientAsync(TcpClient client, string ip)
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

                Console.WriteLine("{0} Connection established\n", imei);

                while (true)
                {
                    if (IsRateLimited(ip))
                    {
                        await Task.Delay(1000); // wait 1 second
                        continue;
                    }
                    int bytesRead = await networkStream.ReadAsync(buffer);

                    if (bytesRead == 0) // client disconnect
                    {
                        Console.WriteLine("\nDisconnected");
                        break;
                    }

                    string rawData = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine("\n Received {0}", rawData);
                    var parsedData = _teltonikaParser.Parse(Convert.FromHexString(rawData));

                    var jsonData = JsonSerializer.Serialize(parsedData);

                    await processDataService.SaveRawRecordAsync(imei, rawData, jsonData); //save to DB

                    Console.WriteLine("\n Received {0}", rawData);
                    
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
                Console.WriteLine("Connection closed.");
                client.Close();
            }
        }
    }
}
