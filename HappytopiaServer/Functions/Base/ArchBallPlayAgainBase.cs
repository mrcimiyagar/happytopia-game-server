using Midopia.HappytopiaServer.Functions.Main;
using Midopia.HappytopiaServer.Models;
using Midopia.HappytopiaServer.Models.Archer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midopia.HappytopiaServer.Functions.Base
{
    public class ArchBallPlayAgainBase : BaseFunc
    {
        public override void process(Session session, long packetCode, string[] args)
        {
            if (session.User == null)
            {
                return;
            }

            if (session.User.CurrentGame != null)
            {
                lock (session.User.CurrentGame)
                {
                    if (session.User.CurrentGame.GetType() == typeof(ArcherUserGame))
                    {
                        ArcherUserGame game = session.User.CurrentGame as ArcherUserGame;

                        lock (game)
                        {
                            if (game.IsEnded)
                            {
                                if (game.User1.Id == session.User.Id)
                                {
                                    if (game.User2Rematch)
                                    {
                                        restartGame(game);
                                    }
                                    else
                                    {
                                        game.User1Rematch = true;
                                    }
                                }
                                else
                                {
                                    if (game.User1Rematch)
                                    {
                                        restartGame(game);
                                    }
                                    else
                                    {
                                        game.User2Rematch = true;
                                    }
                                }
                            }
                        }
                    }
                    else if (session.User.CurrentGame.GetType() == typeof(ArcherBotGame))
                    {
                        ArcherBotGame game = session.User.CurrentGame as ArcherBotGame;

                        lock (game)
                        {
                            if (game.IsEnded)
                            {
                                if (game.User.Id == session.User.Id)
                                {
                                    if (game.BotRematch)
                                    {
                                        restartGame(game);
                                    }
                                    else
                                    {
                                        game.UserRematch = true;
                                    }
                                }
                                else
                                {
                                    if (game.UserRematch)
                                    {
                                        restartGame(game);
                                    }
                                    else
                                    {
                                        game.BotRematch = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void restartGame(ArcherGame game)
        {
            game.restart();
        }
    }
}