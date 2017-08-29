using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Midopia.HappytopiaServer.Models;

namespace Midopia.HappytopiaServer.Functions.Main
{
    public class OnPlaneCatchCoin : BaseFunc
    {
        public override void process(Session session, long packetCode, string[] args)
        {
            if (session.User != null)
            {
                Program.DatabaseManager.increaseUserCoin(session.User.Id, 1 * session.User.StarsCount);
                session.User.Coin += 10 * session.User.StarsCount;

                session.sendPacket("answer", packetCode, new string[0]);
            }
        }
    }
}