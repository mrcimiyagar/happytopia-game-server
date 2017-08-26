using Midopia.HappytopiaServer.Controllers;
using Midopia.HappytopiaServer.Models;

namespace Midopia.HappytopiaServer.Functions.Main
{
    public class GetSpecialOffersData : BaseFunc
    {
        public override void process(Session session, long packetCode, string[] args)
        {
            if (session.User != null)
            {
                string[] result = new string[DatabaseManager.SpecialOffersInts.Length + 2];

                result[0] = DatabaseManager.SpecialOfferTimes[0].ToString();
                result[1] = DatabaseManager.SpecialOfferTimes[1].ToString();

                for (int counter = 2; counter < result.Length; counter++)
                {
                    result[counter] = DatabaseManager.SpecialOffersInts[counter - 2].ToString();
                }

                session.sendPacket("answer", packetCode, result);
            }
        }
    }
}