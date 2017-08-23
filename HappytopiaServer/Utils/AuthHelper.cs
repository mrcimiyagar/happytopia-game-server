using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midopia.HappytopiaServer.Utils
{
    class AuthHelper
    {
        static string keySource = "abcdefghijklmnopqrstuvwxyz0123456789";

        public static string makeKey64()
        {
            string result = "";

            Random rnd = new Random();

            for (int counter = 0; counter < 64; counter++)
            {
                result += keySource[rnd.Next(keySource.Length - 1)];
            }

            return result;
        }
    }
}