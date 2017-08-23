using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Midopia.HappytopiaServer.Models;
using Midopia.HappytopiaServer.Controllers;

namespace Midopia.HappytopiaServer.Functions.Main
{
    public class GetHomeViewsCosts : BaseFunc
    {
        public override void process(Session session, long packetCode, string[] args)
        {
            if (session.User != null)
            {
                string[] result = new string[DatabaseManager.HomeViewsCosts.Length];

                for (int counter = 0; counter < DatabaseManager.HomeViewsCosts.Length; counter++)
                {
                    result[counter] = DatabaseManager.HomeViewsCosts[counter].ToString();
                }

                session.sendPacket("answer", packetCode, result);
            }
        }
    }
}