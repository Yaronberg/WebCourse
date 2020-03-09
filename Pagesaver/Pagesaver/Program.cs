using System;
using System.IO;
using System.Net.Sockets;


namespace Pagesaver
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {


                Console.WriteLine("Введите запрос типа:\n" +
                    "pagesaver selin.in.ua output.html\n" +
                    "pagesaver ua.fm output.html\n" +
                    "pagesaver http://selin.in.ua/solvve/text.txt output.html\n" +
                    "pagesaver http://selin.in.ua/solvve/html.html output.html\n" +
                    "pagesaver http://info.cern.ch/hypertext/WWW/TheProject.html output.html");

                var request = Console.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                string addr = request[1];
                string serverAddr = addr;

                if (addr.IndexOf("http://") >= 0)
                {
                    addr = addr.Substring(7);

                }

                serverAddr = addr.Split("/")[0];

                if (addr.IndexOf("/") != -1)
                {
                    addr = addr.Substring(addr.IndexOf("/"));
                }
                else
                {
                    addr = "/";
                }

                var Path = "..\\..\\..\\" + request[2];

                var message = $"GET {addr} HTTP/1.0\r\nHost: {serverAddr}\r\n\r\n";
                Console.WriteLine("before:" + message);

                try
                {
                    var port = 80;

                    var client = new TcpClient(serverAddr, port);
                    var data = System.Text.Encoding.ASCII.GetBytes(message);
                    NetworkStream stream = client.GetStream();

                    stream.Write(data, 0, data.Length);
                    stream.Flush();
                    Console.WriteLine("Sent {0}", message);

                    var responseData = new byte[65535];
                    int bytesRead = stream.Read(responseData, 0, responseData.Length);
                    var responseMessage = System.Text.Encoding.ASCII.GetString(responseData, 0, bytesRead);
                    Console.WriteLine("Receive {0}\n", responseMessage);
                    stream.Close();
                    client.Close();

                    var indexString = responseMessage.IndexOf("<html>") == -1 ? responseMessage.IndexOf("\r\n") : responseMessage.IndexOf("<html>");
                    responseMessage = responseMessage.Substring(indexString);

                    using (StreamWriter sw = new StreamWriter(Path, false, System.Text.Encoding.Default))
                    {
                        sw.WriteLine(responseMessage);
                    }

                    Console.WriteLine("Запись выполнена");

                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e);
                }

                Console.WriteLine();
            }
        }
    }
}
