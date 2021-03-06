﻿using System;
using System.Net;
using System.Net.Sockets;

namespace serverSolvve
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("simple server");
            try
            {
                var port = 19333;
                var localAddr = IPAddress.Parse("127.0.0.1");
                var server = new TcpListener(localAddr, port);
                server.Start();

                while (true)
                {
                    Console.WriteLine("wating for connection");
                    var client = server.AcceptTcpClient();

                    NetworkStream stream = client.GetStream();

                    string content =
                        "<!DOCTYPE html>\r\n" +
                        "<html>\r\n" +
                        "<head\r\n>" +
                        "<title></title>\r\n" +
                        "</head>\r\n" +
                        "<body>\r\n" +
                        "<h1>Hi, everyone!</h1>\r\n" +
                        "</body>\r\n" +
                        "</html >";

                     string header =
                        "HTTP/1.0 200 OK\r\n" +
                        "Server: SimpleServer\r\n" +
                        "Content-Type: text/html\r\n" +
                        "Content-Length: " + content.Length + "\r\n\r\n";

                    var response = System.Text.Encoding.ASCII.GetBytes(header + content);
                    stream.Write(response, 0, response.Length);

                    Console.WriteLine("sent: \n{0}", header);
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
