using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Midopia.HappytopiaServer.Models;

namespace Midopia.HappytopiaServer.Functions.Main
{
    public class FetchMyUserInfo : BaseFunc
    {
        public override void process(Session session, long packetCode, string[] args)
        {
            if (session.User == null)
            {
                return;
            }

            string[] result = new string[19];

            result[0] = session.User.Name.ToString();
            result[1] = session.User.Coin.ToString();
            result[2] = session.User.Gem.ToString();
            result[3] = session.User.PlaneRecord.ToString();

            for (int counter = 0; counter < 11; counter++)
            {
                result[counter + 4] = session.User.Views[counter] ? "1" : "0";
            }

            for (int counter = 0; counter < 4; counter++)
            {
                result[counter + 15] = session.User.Navas[counter] ? "1" : "0";
            }

            session.sendPacket("answer", packetCode, result);
        }
    }
}