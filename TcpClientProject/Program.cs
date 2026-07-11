using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace TcpClientProject
{
    class Program
    {
        public static TcpClient client = new TcpClient();
        public static NetworkStream stream;
        public static async Task Main(string[] args)
        {
            Connect("123456789010000");

            while (true)
            {
                string? message = Console.ReadLine();

                if (string.IsNullOrEmpty(message)) break;

                await Send(message);
            }
        }

        public static async Task Connect(string deviceImei)
        {
            //string server = "192.168.0.175";
            string server = "127.0.0.1";
            Int32 port = 13000;
            int maxRetries = 5; 
            int waitTimer = 1000; //ms

            while (maxRetries > 0)
            {
                try
                {
                    Console.WriteLine("Connecting...");
                    await client.ConnectAsync(server, port);
                    Console.WriteLine("Connected\n");
                    break;
                }
                catch (SocketException)
                {
                    Console.WriteLine("Server is unreachable \n");
                }
                maxRetries--;
                if (maxRetries > 0)
                {
                    await Task.Delay(waitTimer);
                }
            }  

            stream = client.GetStream();

            //Send IMEI
            byte[] sendData = System.Text.Encoding.UTF8.GetBytes(deviceImei);
            stream.Write(sendData, 0, sendData.Length);
            Console.WriteLine("Sending IMEI...");
            //Receive ACK
            byte[] receiveData = new Byte[256];
            int bytes = stream.Read(receiveData, 0, receiveData.Length);

            if (bytes == 0)
            {
                Console.WriteLine("Connection closed");
                return;
            }

            Console.WriteLine("Handshake complete.\n");

            _ = Task.Run(ReceiveAsync);
        }

        public static async Task Send(string message)
        {
            byte[] sendData = System.Text.Encoding.UTF8.GetBytes(message);

            stream.Write(sendData, 0, sendData.Length);

            Console.WriteLine("Sent: {0}", message);
        }

        public static async Task ReceiveAsync()
        {
            byte[] receiveData = new Byte[256];
            string responseData = string.Empty;

            try
            {
                while (true)
                {
                    Int32 bytes = stream.Read(receiveData, 0, receiveData.Length);

                    if (bytes == 0)
                    {
                        Console.WriteLine("Connection closed");
                        break;
                    }

                    responseData = System.Text.Encoding.UTF8.GetString(receiveData, 0, bytes);

                    Console.WriteLine("Received: {0}", responseData);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error occurred while receiving data.");
                throw;
            }
            
        }
    }
}
