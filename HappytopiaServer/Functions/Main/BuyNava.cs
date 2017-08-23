using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Midopia.HappytopiaServer.Models;

namespace Midopia.HappytopiaServer.Functions.Main
{
    public class BuyNava : BaseFunc
    {
        public override void process(Session session, long packetCode, string[] args)
        {
            if (session.User != null)
            {
                int navaIndex = Convert.ToInt32(args[0]);

                if (navaIndex >= 0 && navaIndex <= 3)
                {
                    if (!session.User.Navas[navaIndex])
                    {
                        if (session.User.Coin >= 30)
                        {
                            Program.DatabaseManager.decreaseUserCoin(session.User.Id, 30);
                            Program.DatabaseManager.enableNavaMode(session.User.Id, navaIndex);

                            session.User.Coin -= 30;
                            session.User.Navas[navaIndex] = true;

                            session.sendPacket("answer", packetCode, new string[] { session.User.Coin.ToString() });
                        }
                    }
                }
            }
        }
    }
}