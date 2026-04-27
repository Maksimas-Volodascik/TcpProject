using System.Configuration;
using System.Net;
using System.Net.Sockets;

namespace TcpClientProject
{
    class Program
    {
        public static void Main(string[] args)
        {
            //Connect("magic");
            IPAddress ip;
            if(!IPAddress.TryParse(ConfigurationManager.AppSettings["ipAddress"], out ip))
            {
                Console.WriteLine("Error parsing IP address");
                ip = IPAddress.Any;
            }

            Console.WriteLine(ip);
        }

        public static void Connect(String message)
        {
            try
            {
                String server = "127.0.0.1";
                Int32 port = 13000;

                using TcpClient client = new TcpClient(server, port);

                // Convert message to ASCII and store it as Byte Array
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(message); //to UTF8 to support non-ascii chars

                // Get a client stream for reading and writing.
                NetworkStream stream = client.GetStream();

                // Send message
                stream.Write(data, 0, data.Length);

                Console.WriteLine("Sent: {0}", message);

                // Receive the response
                data = new Byte[256];

                String responseData = String.Empty;

                // Read the first batch of the TcpServer response bytes
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine("Received: {0}", responseData);

            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
        }
    }
}
