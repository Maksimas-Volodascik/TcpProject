using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TcpListenerProject
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            await ServerListener();
        }

        public static async Task ServerListener()
        {
            TcpListener server;
            Int32 port;
            IPAddress ipAddress;

            if (!IPAddress.TryParse(ConfigurationManager.AppSettings["ipAddress"], out ipAddress))
            {
                ipAddress = IPAddress.Any;
            }

            if (!Int32.TryParse(ConfigurationManager.AppSettings["ipAddress"], out port))
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
            finally{
                server.Stop();
            }
            
            /*
            try
            {
                while (true)
                {
                    TcpClient client = await server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    NetworkStream stream = client.GetStream();

                    Byte[] bytes = new Byte[2048];

                    int bytesRead = stream.Read(bytes);

                    while (client.Connected && bytesRead != 0)
                    {
                        
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, bytesRead);
                        Console.WriteLine("Received {0}", data);

                        // send response
                        string responseMsg = "01";
                        bytes = System.Text.Encoding.ASCII.GetBytes(responseMsg);
                        stream.Write(bytes, 0, bytes.Length);
                        Console.WriteLine("Sent: {0}", responseMsg);
                    }
                    // Loop to receive all the data sent by the client.
                    /*while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        Console.WriteLine("Received {0}", data);

                        // Process the data sent by the client
                        data = data.ToUpper();

                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                        // Send back a response
                        stream.Write(msg, 0, msg.Length);
                        Console.WriteLine("Sent: {0}", data);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                server.Stop();
            }*/

            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }

        public static async Task HandleClientAsync(TcpClient client)
        {
            try
            {
                using (client)
                using (NetworkStream stream = client.GetStream())
                {
                    Console.WriteLine("Connected! {0}", client.Client.RemoteEndPoint);
                    byte[] bytes = new Byte[2048];

                    while (true)
                    {
                        int bytesRead = await stream.ReadAsync(bytes);

                        if (bytesRead == 0) break; // client disconnect

                        string data = System.Text.Encoding.ASCII.GetString(bytes, 0, bytesRead);
                        Console.WriteLine("\n Received {0}", data);

                        // send response
                        string responseMsg = "01";
                        byte[] resp = System.Text.Encoding.ASCII.GetBytes(responseMsg);
                        await stream.WriteAsync(resp, 0, resp.Length);
                        Console.WriteLine("\n Sent: {0} \n", responseMsg);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Socket error: {0}", e.Message);
            }
        }
    }
}
