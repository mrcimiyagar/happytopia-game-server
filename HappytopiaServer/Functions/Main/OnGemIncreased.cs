using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Midopia.HappytopiaServer.Models;

namespace Midopia.HappytopiaServer.Functions.Main
{
    public class OnGemIncreased : BaseFunc
    {
        public override void process(Session session, long packetCode, string[] args)
        {
            if (session.User != null)
            {
                int gem = Convert.ToInt32(args[0]);

                Program.DatabaseManager.increaseUserGem(session.User.Id, gem);
                session.User.Gem += gem;

                session.sendPacket("answer", -1, new string[] { session.User.Gem.ToString() });
            }
        }
    }
}