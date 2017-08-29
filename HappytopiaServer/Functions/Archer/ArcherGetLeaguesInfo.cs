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
    public class ArcherGetLeaguesInfo : BaseFunc
    {
        public override void process(Session session, long packetCode, string[] args)
        {
            if (session.User != null)
            {
                string[] result = new string[ArcherGame.StartCoinCost.Length * 6];

                for (int counter = 0; counter < result.Length; counter += 6)
                {
                    result[counter] = ArcherGame.StartXpNeed[counter / 6].ToString();
                    result[counter + 1] = ArcherGame.StartCoinCost[counter / 6].ToString();
                    result[counter + 2] = ArcherGame.WinXpPrize[counter / 6].ToString();
                    result[counter + 3] = ArcherGame.WinCoinPrize[counter / 6].ToString();
                    result[counter + 4] = ArcherGame.WinGemPrize[counter / 6].ToString();
                    result[counter + 5] = ArcherGame.LooseXpPrize[counter / 6].ToString();
                }

                session.sendPacket("answer", packetCode, result);
            }
        }
    }
}