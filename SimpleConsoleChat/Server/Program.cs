using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new MyServer("127.0.0.1", 50000);
            server.Start();

            Console.ReadLine();
        }
    }

    public class User
    {
        public TcpClient Client { get; set; }
        public string Nickname { get; set; }
    }

    class MyServer
    {
        TcpListener server;
        List<User> users = new List<User>();
        const string StatusOK = "200";
        const string StatusNameBusy = "403"; 

        public MyServer(string ip, ushort port)
        {
            server = new TcpListener(IPAddress.Parse(ip), port);

            //user.Add(new )
            server.Start();
        }


        public void Start()
        {
            Console.WriteLine("server is running...");

            while (true)
            {
                if (server.Pending())
                {
                    TcpClient client = server.AcceptTcpClient();

                    CheckNickname(client);

                }

                foreach (var item in users.Where(x => x.Client.Connected && x.Client.Available > 0))
                {
                    SendDataToAll(item);
                }

                users.RemoveAll(i => !i.Client.Connected);

                Thread.Sleep(20);

            }
        }

        private bool CheckNickname(TcpClient newClient)
        {

            while (true)
            {
                string newUserNick = GetData(newClient);
                if (users.Exists(user => user.Nickname == newUserNick))
                {
                    Console.WriteLine($"nick \"{newUserNick}\" busy");
                    SentData(newClient, StatusNameBusy);
                }
                else
                {

                    SentData(newClient, StatusOK);
                    var newUser = new User { Client = newClient, Nickname = newUserNick };
                    users.Add(newUser);
                    SendDataToAll(newUser, $"\"{newUserNick}\" entered in chat");
                    return true;
                }
            }

        }

        private bool SentData(TcpClient client, string msg)
        {
            try
            {
                var stream = client.GetStream();
                byte[] data = Encoding.UTF8.GetBytes(msg);
                stream.Write(data, 0, data.Length);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void SendDataToAll(User sender)
        {
            string msg = $"{sender.Nickname}: {GetData(sender.Client)}";

            var usersArr = from client 
                           in users 
                           where client.Client != sender.Client && client.Client.Connected 
                           select client.Client;

            foreach (var item in usersArr)
                if (!SentData(item, msg)) item.Close();

        }

        private void SendDataToAll(User sender, string m)
        {
            var usersArr = from client
                           in users
                           where client.Client != sender.Client && client.Client.Connected
                           select client.Client;

            foreach (var item in usersArr)
                if (!SentData(item, m)) item.Close();
        }

        private string GetData(TcpClient client)
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

            Console.WriteLine($"Received: {client.Client.RemoteEndPoint} () => {data}");

            return data.ToString();
        }

    }
}