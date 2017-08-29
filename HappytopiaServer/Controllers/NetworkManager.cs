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

        private List<Session> activeSessions;

        public int SessionsCount
        {
            get
            {
                return this.activeSessions.Count;
            }
        }

        public NetworkManager()
        {
            this.activeSessions = new List<Session>();
            new Thread(connect).Start();
        }

        private void connect()
        {
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse(serverIp), 8080);

            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            serverSocket.Bind(ip);
            serverSocket.Listen(50000);

            while (true)
            {
                Socket clientSocket = serverSocket.Accept();

                Console.WriteLine("new client connected !");

                Session session = new Session(sessionCounter++, clientSocket);
                this.activeSessions.Add(session);
            }
        }

        public void RemoveSession(Session session)
        {
            this.activeSessions.Remove(session);
        }
    }
}