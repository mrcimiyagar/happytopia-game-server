using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Midopia.HappytopiaServer.Models;

namespace Midopia.HappytopiaServer.Functions.Main
{
    public class OnCoinIncreased : BaseFunc
    {
        public override void process(Session session, long packetCode, string[] args)
        {
            if (session.User != null)
            {
                int coin = Convert.ToInt32(args[0]);

                Program.DatabaseManager.increaseUserCoin(session.User.Id, coin);
                session.User.Coin += coin;

                session.sendPacket("answer", -1, new string[] { session.User.Coin.ToString() });
            }
        }
    }
}