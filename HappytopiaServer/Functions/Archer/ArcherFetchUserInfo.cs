
using Midopia.HappytopiaServer.Functions.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Midopia.HappytopiaServer.Models;
using Midopia.HappytopiaServer.Models.Archer;

namespace Midopia.HappytopiaServer.Functions.Archer
{
    public class ArcherFetchUserInfo : BaseFunc
    {
        public override void process(Session session, long packetCode, string[] args)
        {
            int id = Convert.ToInt32(args[0]);

            ArcherUser archerUser = Program.DatabaseManager.getArcherUserInfo(id);
            User user = null;

            if (Program.SharingManager.UsersDic.TryGetValue(id, out user))
            {
                session.sendPacket("answer", packetCode, new string[] { user.ArcherLevel.ToString(), archerUser.XP.ToString(), archerUser.Game.ToString(), archerUser.Won.ToString() });
            }
        }
    }
}