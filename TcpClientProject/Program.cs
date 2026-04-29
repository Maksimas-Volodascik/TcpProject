using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Sockets;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TcpClientProject
{
    class Program
    {
        public static TcpClient client = new TcpClient();
        public static NetworkStream stream;
        public static async Task Main(string[] args)
        {
            Connect();

            while (true)
            {
                string? message = Console.ReadLine();

                if (string.IsNullOrEmpty(message) || message.ToLower().Equals("quit")) break;

                await Send(message);
            }
        }

        public static async Task Connect()
        {
            string server = "127.0.0.1";
            Int32 port = 13000;

            await client.Client.ConnectAsync(server, port);

            stream = client.GetStream();

            Console.WriteLine("Connected.");

            _ = Task.Run(ReceiveAsync);
        }

        public static async Task Send(string message)
        {
            byte[] sendData = System.Text.Encoding.ASCII.GetBytes(message); //to UTF8 to support non-ascii chars

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

                    responseData = System.Text.Encoding.ASCII.GetString(receiveData, 0, bytes);

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
