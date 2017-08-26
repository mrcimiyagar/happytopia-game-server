using Midopia.HappytopiaServer.Models.Archer;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Midopia.HappytopiaServer.Models
{
    public class Session
    {
        public enum SessionStates { NotConnected, ConnectionPending, Connected }

        private int id;
        public int Id { get { return this.id; } }

        private User user;
        public User User
        {
            get
            {
                return this.user;
            }
            set
            {
                this.user = value;

                if (this.user != null)
                {
                    Console.WriteLine("user logged in.");
                }
                else
                {
                    Console.WriteLine("user logged out.");
                }
            }
        }

        private Socket socket;
        public Socket Socket
        {
            get
            {
                return this.socket;
            }
            set
            {
                try {
                    if (value == null)
                    {
                        if (socket != null)
                        {
                            this.socket.Shutdown(SocketShutdown.Both);
                            this.socket.Close();
                        }
                    }
                }
                catch(Exception) { }

                this.socket = value;

                if (this.socket != null)
                {
                    this.writerThread = new Thread((socket) => { initWriter((Socket)socket); });
                    this.readerThread = new Thread((socket) => { initReader((Socket)socket); });

                    this.writerThread.Start(this.socket);
                    this.readerThread.Start(this.socket);

                    netEndThreadCounter++;

                    this.state = SessionStates.Connected;
                }
                else
                {
                    this.state = SessionStates.NotConnected;
                }
            }
        }

        private SessionStates state;
        public SessionStates State { get { return this.state; } set { this.state = value; } }

        private Thread writerThread;
        public Thread WriterThread { get { return this.writerThread; } set { this.writerThread = value; } }

        private Thread readerThread;
        public Thread ReaderThread { get { return this.readerThread; } set { this.readerThread = value; } }

        private BlockingCollection<string> packetCollection;

        private int netEndThreadCounter = 0;
        private object netEndThreadLockObj = new object();
        bool sessionBreaking = false;

        private long lastPacketCode = -2;
        public long LastPacketCode { set { this.lastPacketCode = value; } }

        public Session(int id, Socket socket)
        {
            this.id = id;
            this.user = null;
            this.writerThread = null;
            this.readerThread = null;
            this.packetCollection = new BlockingCollection<string>();
            this.state = SessionStates.NotConnected;
            this.Socket = socket;
        }

        public void sendPacket(string function, long packetCode, string[] args)
        {
            string packet = packetCode + "," + function;

            foreach (string arg in args)
            {
                packet += "," + arg;
            }

            this.packetCollection.Add(packet);
        }

        public void forcePacket(string function, long packetCode, string[] args)
        {
            string packet = packetCode + "," + function;

            foreach (string arg in args)
            {
                packet += "," + arg;
            }

            lock (this.socket)
            {
                this.socket.Send(Encoding.UTF8.GetBytes(packet));
            }
        }

        public void initWriter(Socket socket)
        {
            string packet = null;

            try
            {
                while (true)
                {
                    packet = this.packetCollection.Take();

                    lock (this.socket)
                    {
                        socket.Send(Encoding.UTF8.GetBytes(packet + "|"));
                    }

                    if (packet.StartsWith(this.lastPacketCode + ","))
                    {
                        this.handleSessionClose();
                    }
                }
            }
            catch (Exception ignored)
            {
                if (packet != null)
                {
                    lock (this)
                    {
                        BlockingCollection<string> leftPackets = new BlockingCollection<string>();

                        leftPackets.Add(packet);
                        
                        foreach (string leftPacket in packetCollection)
                        {
                            leftPackets.Add(leftPacket);
                        }

                        this.packetCollection = leftPackets;
                    }
                }

                try
                {
                    readerThread.Interrupt();
                }
                catch (Exception)
                {

                }

                try
                {
                    writerThread.Interrupt();
                }
                catch (Exception)
                {

                }
            }
            finally
            {
                handleSessionClose();
            }
        }

        public void initReader(Socket socket)
        {
            try {
                byte[] receivedByteArr = new byte[16384];

                while (true)
                {
                    int length = socket.Receive(receivedByteArr);
                    char[] receivedArr = new char[length];
                    int charLen = Encoding.UTF8.GetDecoder().GetChars(receivedByteArr, 0, length, receivedArr, 0);
                    string received = new string(receivedArr);

                    received = received.Substring(0, received.Length - 1);

                    string[] packetsReceived = received.Split('|');

                    foreach (string recv in packetsReceived)
                    {
                        //Console.WriteLine("packet received : " + recv);

                        string[] parts = recv.Split(',');

                        long packetCode = Convert.ToInt64(parts[0]);

                        string function = parts[1];

                        string[] args = new string[parts.Length - 2];

                        for (int counter = 0; counter < args.Length; counter++)
                        {
                            args[counter] = parts[counter + 2];
                        }

                        Program.PacketManager.process(this, function, packetCode, args);
                    }
                }
            }
            catch (Exception ignored)
            {
                try
                {
                    readerThread.Interrupt();
                }
                catch (Exception)
                {

                }

                try
                {
                    writerThread.Interrupt();
                }
                catch (Exception)
                {

                }
            }
            finally
            {
                handleSessionClose();
            }
        }

        private void handleSessionClose()
        {
            if (!sessionBreaking)
            {
                lock (netEndThreadLockObj)
                {
                    try
                    {
                        if (socket != null)
                        {
                            this.socket.Shutdown(SocketShutdown.Both);
                            this.socket.Close();
                        }
                    }
                    catch (Exception) { }

                    this.Socket = null;
                    this.state = SessionStates.ConnectionPending;

                    netEndThreadCounter++;

                    new NetEndFunc(netEndThreadCounter, new Thread((threadId) =>
                    {
                        Thread.Sleep(10000);

                        if (((int)threadId) == netEndThreadCounter)
                        {
                            try
                            {
                                if (this.user != null)
                                {
                                    this.user.IsOnline = false;
                                    this.user.CurrentSession = null;

                                    if (this.user.GameStartReq != null)
                                    {
                                        this.user.GameStartReq.cancelRequest();
                                        this.user.GameStartReq = null;
                                    }

                                    if (this.user.CurrentGame != null)
                                    {
                                        Game game = this.user.CurrentGame;

                                        if (game.GetType() == typeof(ArcherUserGame))
                                        {
                                            ((ArcherUserGame)game).User1.CurrentGame = null;
                                            ((ArcherUserGame)game).User2.CurrentGame = null;
                                            ((ArcherUserGame)game).gameForceEnd(this.user);
                                        }
                                        else if (game.GetType() == typeof(ArcherBotGame))
                                        {
                                            ((ArcherBotGame)game).User.CurrentGame = null;
                                            ((ArcherBotGame)game).gameForceEnd(this.user);
                                        }

                                        Program.GameManager.killGame(game);
                                    }

                                    this.User = null;
                                }

                                this.writerThread = null;
                                this.readerThread = null;
                            }
                            catch (Exception ignored) { Console.WriteLine(ignored.ToString()); }

                            try
                            {
                                Program.NetworkManager.RemoveSession(this);
                            }
                            catch (Exception) { }
                        }
                    }));
                }
            }
        }

        public void detachSocket()
        {
            sessionBreaking = true;

            try
            {
                writerThread.Interrupt();
            }
            catch (Exception)
            {

            }

            try
            {
                readerThread.Interrupt();
            }
            catch (Exception)
            {

            }

            this.Socket = null;
            this.readerThread = null;
            this.writerThread = null;
        }

        private void handleSessionCloseOrder()
        {
            try
            {
                if (socket != null)
                {
                    this.socket.Shutdown(SocketShutdown.Both);
                    this.socket.Close();
                }
            }
            catch (Exception) { }

            this.Socket = null;

            try
            {
                if (this.user != null)
                {
                    this.user.IsOnline = false;
                    this.user.CurrentSession = null;

                    if (this.user.GameStartReq != null)
                    {
                        this.user.GameStartReq.cancelRequest();
                        this.user.GameStartReq = null;
                    }

                    if (this.user.CurrentGame != null)
                    {
                        Game game = this.user.CurrentGame;

                        if (game.GetType() == typeof(ArcherUserGame))
                        {
                            ((ArcherUserGame)game).User1.CurrentGame = null;
                            ((ArcherUserGame)game).User2.CurrentGame = null;
                            ((ArcherUserGame)game).gameForceEnd(this.user);
                        }
                        else if (game.GetType() == typeof(ArcherBotGame))
                        {
                            ((ArcherBotGame)game).User.CurrentGame = null;
                            ((ArcherBotGame)game).gameForceEnd(this.user);
                        }

                        Program.GameManager.killGame(game);
                    }

                    this.User = null;
                }

                this.writerThread = null;
                this.readerThread = null;
            }
            catch (Exception ignored) { Console.WriteLine(ignored.ToString()); }
        }

        public void attachSocket(Socket socket)
        {
            this.netEndThreadCounter++;
            this.Socket = socket;
        }

        public void detachUser()
        {
            if (this.user != null)
            {
                this.user.IsOnline = false;
                this.user.CurrentSession = null;

                if (this.user.GameStartReq != null)
                {
                    this.user.GameStartReq.cancelRequest();
                    this.user.GameStartReq = null;
                }

                if (this.user.CurrentGame != null)
                {
                    Game game = this.user.CurrentGame;

                    if (game.GetType() == typeof(ArcherUserGame))
                    {
                        ((ArcherUserGame)game).User1.CurrentGame = null;
                        ((ArcherUserGame)game).User2.CurrentGame = null;
                        ((ArcherUserGame)game).gameForceEnd(this.user);
                    }
                    else if (game.GetType() == typeof(ArcherBotGame))
                    {
                        ((ArcherBotGame)game).User.CurrentGame = null;
                        ((ArcherBotGame)game).gameForceEnd(this.user);
                    }

                    Program.GameManager.killGame(game);
                }

                this.User = null;
            }
        }
    }

    class NetEndFunc
    {
        private int id;
        public int Id { get { return this.id; } }

        private Thread netEndThread;
        public Thread NetEndThread { get { return this.netEndThread; } }

        public NetEndFunc(int id, Thread netEndThread)
        {
            this.id = id;
            this.netEndThread = netEndThread;
            this.netEndThread.Start(id);
        }
    }
}