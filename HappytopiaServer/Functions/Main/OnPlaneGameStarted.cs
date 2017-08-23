using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Midopia.HappytopiaServer
    .Models;

namespace Midopia.HappytopiaServer.Functions.Main
{
    public class OnPlaneGameStarted : BaseFunc
    {
        public override void process(Session session, long packetCode, string[] args)
        {
            if (session.User != null)
            {
                Program.DatabaseManager.decreaseUserCoin(session.User.Id, 3);
                session.User.Coin -= 3;

                session.sendPacket("answer", packetCode, new string[0]);
            }
        }
    }
}