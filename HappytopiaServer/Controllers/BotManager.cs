using Midopia.HappytopiaServer.Models;
using Midopia.HappytopiaServer.Models.Archer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midopia.HappytopiaServer.Controllers
{
    public class BotManager
    {
        public BotManager()
        {

        }

        public ArcherBot prepareArcherBot()
        {
            return new ArcherBot(0, Program.DatabaseManager.getRandomBotName());
        }

        public void killBot(Bot bot)
        {
            
        }
    }
}