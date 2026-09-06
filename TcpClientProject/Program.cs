using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TcpClientProject
{
    class Program
    {
        public static TcpClient client = new TcpClient();
        public static NetworkStream stream;
        public static async Task Main(string[] args)
        {
            DateTimeOffset dateTime = new DateTimeOffset(2026, 2, 9, 15, 30, 25, TimeSpan.Zero);
            PacketBuilder PB = new PacketBuilder(dateTime);
 
            if (await Connect("000F313233343536373839303130303030"))
            {
                try
                {
                    Console.WriteLine("Upload type: \n 1. Manual \n 2. File");
                    var option = Console.ReadLine();
                    if (option == "1")
                    {
                        while (true)
                        {
                            var codec = Console.ReadLine();
                            await Send(codec);
                        }
                    }
                    else if(option == "2")
                    {
                        using (StreamReader sr = File.OpenText("Coordinates.txt")) // contains rows of longitude,latitude
                        {
                            string s = "";
                            while ((s = sr.ReadLine()) != null)
                            {
                                string[] coords = s.Split(',');
                                //Console.WriteLine(PB.GetCodecString(Double.Parse(coords[0]), Double.Parse(coords[1])));
                                await Task.Delay(1000);
                                await Send(PB.GetCodecString(Double.Parse(coords[0]), Double.Parse(coords[1])));
                            }
                        }
                    } 
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            else
            {
                Console.WriteLine("Nothing happened");
            }
        }

        public static async Task<bool> Connect(string deviceImei)
        {
            //string server = "192.168.0.175"; Docker
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
                else
                {
                    return false;
                }
            }  

            stream = client.GetStream();

            //Send IMEI
            byte[] sendData = System.Text.Encoding.ASCII.GetBytes(deviceImei);
            stream.Write(sendData, 0, sendData.Length);
            Console.WriteLine("Sending IMEI...");
            //Receive ACK
            byte[] receiveData = new Byte[256];
            int bytes = stream.Read(receiveData, 0, receiveData.Length);

            if (bytes == 0)
            {
                Console.WriteLine("Connection closed");
                return false;
            }

            Console.WriteLine("Handshake complete.\n");

            _ = Task.Run(ReceiveAsync);
            return true;
        }

        public static async Task Send(string message)
        {
            byte[] sendData = System.Text.Encoding.ASCII.GetBytes(message);

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
