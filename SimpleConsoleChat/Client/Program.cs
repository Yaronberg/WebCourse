using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MessengerClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var msgClient = new MyTcpMsgClient("127.0.0.1", 50000);
            msgClient.MainLoop();

        }
    }
    class MyTcpMsgClient
    {
        readonly TcpClient client;
        string nickname;


        public MyTcpMsgClient(string ip, int port)
        {
            client = new TcpClient(ip, port);
        }

        public void MainLoop()
        {

            while (!SignInChat());

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    Console.Write(nickname + ": ");

                    string msg = Console.ReadLine();

                    if (msg == ".quit")
                        break;
                    else if (SentToServer(msg))
                    Console.WriteLine("message send successful");
                }

                if (client.Available > 0)
                {
                    Console.WriteLine(ReadFromServer());
                }
                
                Thread.Sleep(20);
            }
        }

        private bool SignInChat()
        {
            Console.Write("Enter your nickname: ");
            while (true)
            {
                this.nickname = Console.ReadLine();
                SentToServer(this.nickname);
                string status = ReadFromServer();

                if (status == "200")
                {
                    Console.WriteLine("Registration successful. You connected to chat");
                    Console.WriteLine($"\"{nickname}\" entered in chat");
                    return true;
                }
                else if (status == "403")
                {
                    Console.WriteLine("That nickname is busy. Try enter another nickname: ");
                    return false;
                }
                else
                {
                    Console.WriteLine("Failed to connect to server");
                }
            } 
            
        }

        private bool SentToServer(string msg)
        {
            try
            {

                var networkStream = client.GetStream();
                var data = Encoding.UTF8.GetBytes(msg);
                networkStream.Write(data, 0, data.Length);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        private string ReadFromServer()
        {
            var stream = client.GetStream();

            byte[] buffer = new byte[256];
            var size = 0;
            StringBuilder data = new StringBuilder();

            do
            {
                size = stream.Read(buffer, 0, buffer.Length);
                data.Append(Encoding.UTF8.GetString(buffer, 0, size));
            }
            while (stream.DataAvailable);

            return data.ToString();
        }

    }
}