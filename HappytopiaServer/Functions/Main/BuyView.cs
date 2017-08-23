using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Midopia.HappytopiaServer
    .Models;
using Midopia.HappytopiaServer.Controllers;

namespace Midopia.HappytopiaServer.Functions.Main
{
    public class BuyView : BaseFunc
    {
        public override void process(Session session, long packetCode, string[] args)
        {
            if (session.User != null)
            {
                int viewIndex = Convert.ToInt32(args[0]);
                

                if (viewIndex >= 0 && viewIndex <= 10)
                {
                    if (!session.User.Views[viewIndex])
                    {
                        int viewPrice = 0;

                        Console.WriteLine("view " + viewIndex + " unlocked !");

                        viewPrice = DatabaseManager.HomeViewsCosts[viewIndex];

                        if (session.User.Coin >= viewPrice)
                        {
                            Program.DatabaseManager.decreaseUserCoin(session.User.Id, viewPrice);
                            Program.DatabaseManager.enableHomeView(session.User.Id, viewIndex);

                            session.User.Coin -= viewPrice;
                            session.User.Views[viewIndex] = true;

                            session.sendPacket("answer", packetCode, new string[] { "ok", session.User.Coin.ToString(), session.User.Gem.ToString() });
                        }
                        else
                        {
                            session.sendPacket("answer", packetCode, new string[] { "error" });
                        }
                    }
                }
            }
        }
    }
}