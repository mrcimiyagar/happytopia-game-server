using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Midopia.HappytopiaServer.Models;

namespace Midopia.HappytopiaServer.Functions.Main
{
    public class OnKababiCollected : BaseFunc
    {
        public override void process(Session session, long packetCode, string[] args)
        {
            if (session.User != null)
            {
                int coins = Convert.ToInt32(args[0]);

                Program.DatabaseManager.increaseUserCoin(session.User.Id, coins * session.User.StarsCount);
                session.User.Coin += coins * session.User.StarsCount;

                session.sendPacket("answer", packetCode, new string[0]);
            }
        }
    }
}