using Midopia.HappytopiaServer.Functions.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Midopia.HappytopiaServer.Models;
using System.Threading;
using System.Timers;
using Midopia.HappytopiaServer.Models.Archer;
using System.IO;

namespace Midopia.HappytopiaServer.Functions.Archer
{
    class ArcherStartGame : BaseFunc
    {
        private GameStartReqHolder[] singleRequest;

        private System.Timers.Timer[] singleWaitingTimer;

        public ArcherStartGame()
        {
            this.singleRequest = new GameStartReqHolder[5];

            for (int counter = 0; counter < 5; counter++)
            {
                this.singleRequest[counter] = new GameStartReqHolder();
            }

            this.singleWaitingTimer = new System.Timers.Timer[5];

            this.singleWaitingTimer[0] = new System.Timers.Timer(10000);
            this.singleWaitingTimer[0].Elapsed += new ElapsedEventHandler((sender, e) => waitingTimerTick(sender, e, 0));
            this.singleWaitingTimer[0].AutoReset = false;

            this.singleWaitingTimer[1] = new System.Timers.Timer(10000);
            this.singleWaitingTimer[1].Elapsed += new ElapsedEventHandler((sender, e) => waitingTimerTick(sender, e, 1));
            this.singleWaitingTimer[1].AutoReset = false;

            this.singleWaitingTimer[2] = new System.Timers.Timer(10000);
            this.singleWaitingTimer[2].Elapsed += new ElapsedEventHandler((sender, e) => waitingTimerTick(sender, e, 2));
            this.singleWaitingTimer[2].AutoReset = false;

            this.singleWaitingTimer[3] = new System.Timers.Timer(10000);
            this.singleWaitingTimer[3].Elapsed += new ElapsedEventHandler((sender, e) => waitingTimerTick(sender, e, 3));
            this.singleWaitingTimer[3].AutoReset = false;

            this.singleWaitingTimer[4] = new System.Timers.Timer(10000);
            this.singleWaitingTimer[4].Elapsed += new ElapsedEventHandler((sender, e) => waitingTimerTick(sender, e, 4));
            this.singleWaitingTimer[4].AutoReset = false;
        }

        private void waitingTimerTick(object sender, ElapsedEventArgs e, int i)
        {
            int timerIndex = i;

            lock (singleRequest[timerIndex])
            {
                if (singleRequest[timerIndex] != null && singleRequest[timerIndex].CurrentRequest.Requester != null)
                {
                    User user = singleRequest[timerIndex].CurrentRequest.Requester;
                    ArcherBot bot = Program.BotManager.prepareArcherBot();

                    singleRequest[timerIndex].CurrentRequest = null;

                    ArcherBotGame game = Program.GameManager.prepareArcherBotGame(user, bot);

                    user.CurrentGame = game;
                    bot.CurrentGame = game;

                    user.GameStartReq = null;

                    game.start(i + 1);
                }
            }
        }

        public override void process(Session session, long packetCode, string[] args)
        {
            if (session.User != null)
            {
                int userRequestedLevel = Convert.ToInt32(args[0]);
                int coinNotNeeded = Convert.ToInt32(args[1]);

                int userLevel = session.User.ArcherLevel;

                if (userRequestedLevel <= userLevel)
                {
                    if (coinNotNeeded == 1 || session.User.Coin >= ArcherGame.getArcherGameCost(userRequestedLevel))
                    {
                        if (coinNotNeeded == 1)
                        {
                            File.AppendAllLines("ArchersPlays", new string[] { session.User.Id.ToString() + ", By Tapsell" });
                        }
                        else
                        {
                            File.AppendAllLines("ArchersPlays", new string[] { session.User.Id.ToString() + ", By Coin" });
                        }

                        session.sendPacket("answer", packetCode, new string[] { "ok" });

                        lock (singleRequest[userRequestedLevel - 1])
                        {
                            if (singleRequest[userRequestedLevel - 1].CurrentRequest != null)
                            {
                                if (singleRequest[userRequestedLevel - 1].CurrentRequest.Requester != null)
                                {
                                    if (singleRequest[userRequestedLevel - 1].CurrentRequest.Requester.Id != session.User.Id)
                                    {
                                        if (singleRequest[userRequestedLevel - 1].CurrentRequest.Requester.CurrentSession != null)
                                        {
                                            singleWaitingTimer[userRequestedLevel - 1].Stop();

                                            User user1 = this.singleRequest[userRequestedLevel - 1].CurrentRequest.Requester;
                                            this.singleRequest[userRequestedLevel - 1].CurrentRequest = null;
                                            User user2 = session.User;

                                            ArcherUserGame game = Program.GameManager.prepareArcherUserGame(user1, user2);
                                            user1.CurrentGame = game;
                                            user2.CurrentGame = game;
                                            game.start(userRequestedLevel);
                                        }
                                        else
                                        {
                                            handleNewSingleRequestSet(session, userRequestedLevel);
                                        }
                                    }
                                }
                                else
                                {
                                    handleNewSingleRequestSet(session, userRequestedLevel);
                                }
                            }
                            else
                            {
                                handleNewSingleRequestSet(session, userRequestedLevel);
                            }
                        }
                    }
                    else
                    {
                        session.sendPacket("answer", packetCode, new string[] { "error", "gold_not_enough" });
                    }
                }
            }
        }

        private void handleNewSingleRequestSet(Session session, int userRequestedLevel)
        {
            singleWaitingTimer[userRequestedLevel - 1].Stop();

            GameStartReq gameStartReq = new GameStartReq(session.User);
            session.User.GameStartReq = gameStartReq;
            this.singleRequest[userRequestedLevel - 1].CurrentRequest = gameStartReq;

            singleWaitingTimer[userRequestedLevel - 1].Start();
        }
    }
}