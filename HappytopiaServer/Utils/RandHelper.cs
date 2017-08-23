using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midopia.HappytopiaServer.Utils
{
    public class RandHelper
    {
        private static Random rnd = new Random();

        public static int makeRandom(int min, int max)
        {
            return rnd.Next(min, max);
        }

        public static float makeRandomFloat(float min, float max)
        {
            return (float)((rnd.NextDouble() * (max - min)) + min);
        }
    }
}
