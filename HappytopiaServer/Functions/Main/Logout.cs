using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Midopia.HappytopiaServer.Models;

namespace Midopia.HappytopiaServer.Functions.Main
{
    class Logout : BaseFunc
    {
        public override void process(Session session, long packetCode, string[] args)
        {
            if (session.User != null)
            {
                User user = session.User;

                session.detachUser();

                user.IsOnline = false;
                user.CurrentSession = null;
                user.CurrentGame = null;

                session.LastPacketCode = packetCode;

                session.sendPacket("answer", packetCode, new string[0]);
            }
        }
    }
}
