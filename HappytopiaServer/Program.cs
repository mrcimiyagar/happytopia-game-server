using Midopia.HappytopiaServer.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Midopia.HappytopiaServer
{
    class Program
    {
        private static DatabaseManager dbaseManager;
        public static DatabaseManager DatabaseManager { get { return dbaseManager; } set { dbaseManager = value; } }

        private static BotManager botManager;
        public static BotManager BotManager { get { return botManager; } set { botManager = value; } }

        private static GameManager gameManager;
        public static GameManager GameManager { get { return gameManager; } set { gameManager = value; } }

        private static SharingManager sharingManager;
        public static SharingManager SharingManager { get { return sharingManager; } }

        private static PacketManager packetManager;
        public static PacketManager PacketManager { get { return packetManager; } set { packetManager = value; } }

        private static NetworkManager netManager;
        public static NetworkManager NetworkManager { get { return netManager; } set { netManager = value; } }

        static void Main(string[] args)
        {
            Console.Title = "Server";

            dbaseManager = new DatabaseManager();
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);

            botManager = new BotManager();
            gameManager = new GameManager();
            sharingManager = new SharingManager();
            packetManager = new PacketManager();
            netManager = new NetworkManager();

            Console.WriteLine("server started.");
        }

        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            dbaseManager.closeDBConnection();
        }
    }
}