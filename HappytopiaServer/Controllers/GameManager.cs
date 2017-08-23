using Midopia.HappytopiaServer.Models;
using Midopia.HappytopiaServer.Models.Archer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midopia.HappytopiaServer.Controllers
{
    class GameManager
    {
        public GameManager()
        {

        }

        public ArcherUserGame prepareArcherUserGame(User user1, User user2)
        {
            ArcherUserGame game = new ArcherUserGame(0);

            game.User1 = user1;
            game.User2 = user2;

            return game;
        }

        public ArcherBotGame prepareArcherBotGame(User user, ArcherBot bot)
        {
            ArcherBotGame game = new ArcherBotGame(0);

            game.User = user;
            game.Bot = bot;

            return game;
        }

        public void killGame(Game rawGame)
        {
            rawGame.clearPlayers();
        }
    }
}