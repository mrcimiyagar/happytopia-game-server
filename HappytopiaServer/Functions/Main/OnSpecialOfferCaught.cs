using Midopia.HappytopiaServer.Functions.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Midopia.HappytopiaServer.Models;
using Midopia.HappytopiaServer;

namespace Midopia.HappytopiaServer.Functions.Main
{
    public class OnSpecialOfferCaught : BaseFunc
    {
        public override void process(Session session, long packetCode, string[] args)
        {
            if (session.User != null)
            {
                int coin = Convert.ToInt32(args[0]);

                Program.DatabaseManager.increaseUserCoin(session.User.Id, coin);
                session.User.Coin += coin;
            }
        }
    }
}