using Midopia.HappytopiaServer.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Midopia.HappytopiaServer.Controllers
{
    class NetworkManager
    {
        private string serverIp = "185.126.202.232";

        private int sessionCounter = 0;

        public NetworkManager()
        {
            new Thread(connect).Start();
        }

        public void connect()
        {
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse(serverIp), 7070);

            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            serverSocket.Bind(ip);
            serverSocket.Listen(50000);

            while (true)
            {
                Socket clientSocket = serverSocket.Accept();

                Console.WriteLine("new client connected !");

                Session session = new Session(sessionCounter++, clientSocket);
            }
        }
    }
}