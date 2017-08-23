using Midopia.HappytopiaServer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;

namespace Midopia.HappytopiaServer.Models.Archer
{
    public class ArcherBot : Bot
    {
        private ArcherBotGame currentGame;
        public ArcherBotGame CurrentGame { get { return this.currentGame; } set { this.currentGame = value; } }

        private int leftBullets;

        private bool botStopped;

        public ArcherBot(int id, string name)
        {
            this.Id = id;
            this.Name = name;
            this.currentGame = null;
        }

        public void startBot(ArcherBotGame game)
        {
            this.botStopped = false;
            this.currentGame = game;
            this.leftBullets = 3;
        }

        public void stopBot()
        {
            this.botStopped = true;
            this.leftBullets = 0;
            this.currentGame = null;
        }

        public void notifyNewSubGameStarted()
        {
            this.leftBullets = 3;

            new Thread(prepareShootFunc).Start();
        }

        public void notifyFinalSubGameStarted()
        {
            this.leftBullets = 1;

            new Thread(prepareFinalShootFunc).Start();
        }

        private void prepareShootFunc()
        {
            try
            {
                while (this.leftBullets > 0 && !botStopped)
                {
                    int shootWaitTime = RandHelper.makeRandom(3, 5) * 1000;

                    Console.WriteLine(shootWaitTime);

                    Thread.Sleep(shootWaitTime);

                    if (!botStopped)
                    {
                        switch (this.CurrentGame.GameLeagueLevel)
                        {
                            case 1:
                                {
                                    currentGame.botShot(RandHelper.makeRandom(4, 8), RandHelper.makeRandom(0, 360));
                                    break;
                                }
                            case 2:
                                {
                                    currentGame.botShot(RandHelper.makeRandom(6, 9), RandHelper.makeRandom(0, 360));
                                    break;
                                }
                            case 3:
                                {
                                    currentGame.botShot(RandHelper.makeRandom(7, 10), RandHelper.makeRandom(0, 360));
                                    break;
                                }
                            case 4:
                                {
                                    currentGame.botShot(RandHelper.makeRandom(8, 10), RandHelper.makeRandom(0, 360));
                                    break;
                                }
                            case 5:
                                {
                                    currentGame.botShot(RandHelper.makeRandom(9, 10), RandHelper.makeRandom(0, 360));
                                    break;
                                }
                        }

                        this.leftBullets--;
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }

        private void prepareFinalShootFunc()
        {
            try {
                while (this.leftBullets > 0 && !botStopped)
                {
                    int shootWaitTime = RandHelper.makeRandom(3, 5) * 1000;

                    Console.WriteLine(shootWaitTime);

                    Thread.Sleep(shootWaitTime);

                    if (!botStopped)
                    {
                        currentGame.botFinalShot(RandHelper.makeRandomFloat(0.01f, 0.5f), RandHelper.makeRandom(0, 360));

                        this.leftBullets--;
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }
    }
}