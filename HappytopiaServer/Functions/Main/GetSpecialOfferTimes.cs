using Midopia.HappytopiaServer.Controllers;
using Midopia.HappytopiaServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midopia.HappytopiaServer.Functions.Main
{
    public class GetSpecialOfferTimes : BaseFunc
    {
        public override void process(Session session, long packetCode, string[] args)
        {
            if (session.User != null)
            {
                session.sendPacket("answer", packetCode, new string[] { DatabaseManager.SpecialOfferTimes[0].ToString(), DatabaseManager.SpecialOfferTimes[1].ToString() });
            }
        }
    }
}