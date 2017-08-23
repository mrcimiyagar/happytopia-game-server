
using Midopia.HappytopiaServer.Functions.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Midopia.HappytopiaServer.Models;
using System.Globalization;
using Midopia.HappytopiaServer.Models.Archer;

namespace Midopia.HappytopiaServer.Functions.Archer
{
    public class ArcherShootBullet : BaseFunc
    {
        public override void process(Session session, long packetCode, string[] args)
        {
            try
            {
                if (session.User != null)
                {
                    if (session.User.CurrentGame != null)
                    {
                        ArcherGame game = session.User.CurrentGame as ArcherGame;

                        float y = float.Parse(args[0], CultureInfo.InvariantCulture.NumberFormat);
                        float z = float.Parse(args[1], CultureInfo.InvariantCulture.NumberFormat);

                        int score = Convert.ToInt32(args[2]);

                        lock (game)
                        {
                            game.userShot(session, packetCode, y, z, score);
                        }
                    }
                }
            }
            catch (Exception ignored)
            {
                Console.WriteLine(ignored.ToString());
            }
        }
    }
}