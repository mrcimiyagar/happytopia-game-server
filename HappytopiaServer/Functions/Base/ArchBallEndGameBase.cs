
using Midopia.HappytopiaServer.Functions.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Midopia.HappytopiaServer.Models;
using Midopia.HappytopiaServer.Models.Archer;
namespace Midopia.HappytopiaServer.Functions.Base
{
    class ArchBallEndGameBase : BaseFunc
    {
        public override void process(Session session, long packetCode, string[] args)
        {
            try {
                lock (session)
                {
                    if (session.User == null)
                    {
                        return;
                    }

                    if (session.User.GameStartReq != null)
                    {
                        session.User.GameStartReq.cancelRequest();
                        session.User.GameStartReq = null;

                        if (session.User.CurrentGame == null)
                        {
                            session.sendPacket("answer", packetCode, new string[0]);
                            return;
                        }
                    }

                    if (session.User.CurrentGame == null)
                    {
                        session.sendPacket("answer", packetCode, new string[0]);
                        return;
                    }

                    lock (session.User.CurrentGame)
                    {
                        if (session.User.CurrentGame.GetType() == typeof(ArcherUserGame))
                        {
                            ArcherUserGame game = session.User.CurrentGame as ArcherUserGame;

                            game.User1.CurrentGame = null;
                            game.User2.CurrentGame = null;

                            if (!game.IsEnded)
                            {
                                game.gameForceEnd(session.User);
                            }
                            else
                            {
                                game.notifyOpponentLeft(session.User.Id);
                            }
                        }
                        else if (session.User.CurrentGame.GetType() == typeof(ArcherBotGame))
                        {
                            ArcherBotGame game = session.User.CurrentGame as ArcherBotGame;

                            game.User.CurrentGame = null;

                            if (!game.IsEnded)
                            {
                                game.gameForceEnd(session.User);
                            }
                        }
                    }
                    
                    session.sendPacket("answer", packetCode, new string[] { session.User.Coin.ToString(), session.User.Gem.ToString() });
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}