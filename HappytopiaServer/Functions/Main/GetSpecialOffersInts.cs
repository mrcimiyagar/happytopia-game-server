using Midopia.HappytopiaServer.Functions.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Midopia.HappytopiaServer.Models;
using Midopia.HappytopiaServer.Controllers;

namespace Midopia.HappytopiaServer.Functions.Main
{
    public class GetSpecialOffersInts : BaseFunc
    {
        public override void process(Session session, long packetCode, string[] args)
        {
            if (session.User != null)
            {
                string[] result = new string[DatabaseManager.SpecialOffersInts.Length];

                for (int counter = 0; counter < result.Length; counter++)
                {
                    result[counter] = DatabaseManager.SpecialOffersInts[counter].ToString();
                }

                session.sendPacket("answer", packetCode, result);
            }
        }
    }
}