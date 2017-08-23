using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Midopia.HappytopiaServer.Models;

namespace Midopia.HappytopiaServer.Functions.Main
{
    public class FetchGameVersion : BaseFunc
    {
        public override void process(Session session, long packetCode, string[] args)
        {
            session.sendPacket("answer", packetCode, new string[] { Program.DatabaseManager.getGameVersion() });
        }
    }
}