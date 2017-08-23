using Midopia.HappytopiaServer.Functions.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Midopia.HappytopiaServer.Models;
using Midopia.HappytopiaServer.Utils;
using System.Text.RegularExpressions;

namespace Midopia.HappytopiaServer.Functions.Main
{
    class Register : BaseFunc
    {
        public override void process(Session session, long packetCode, string[] args)
        {
            string name = args[0];

            if (name.Length > 0 && Regex.IsMatch(name, "^[a-zA-Z0-9]*$"))
            {
                string password = AuthHelper.makeKey64();

                User user = Program.DatabaseManager.createUser(password, name);

                Program.SharingManager.UsersDic.Add(user.Id, user);

                session.sendPacket("answer", packetCode, new string[] { user.Id.ToString(), user.Password });
            }
        }
    }
}