using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Midopia.HappytopiaServer.Models;

namespace Midopia.HappytopiaServer.Functions.Main
{
    class Login : BaseFunc
    {
        public override void process(Session session, long packetCode, string[] args)
        {
            if (session.User == null)
            {
                int id = Convert.ToInt32(args[0]);
                string password = args[1];

                lock (Program.SharingManager.UsersDic)
                {
                    User user = null;

                    if (Program.SharingManager.UsersDic.TryGetValue(id, out user))
                    {
                        if (user.Password == password)
                        {
                            if (user.CurrentSession == null)
                            {
                                Console.WriteLine("logging in as new session !");
                                
                                user.IsOnline = true;
                                session.User = user;
                                user.CurrentSession = session;

                                session.sendPacket("answer", packetCode, new string[] { "ok", user.Id.ToString(), user.Name });
                                
                                return;
                            }
                            else
                            {
                                Console.WriteLine("logging in to existing session !");

                                session.forcePacket("answer", packetCode, new string[] { "ok", user.Id.ToString(), user.Name });

                                user.CurrentSession.attachSocket(session.Socket);
                                session.detachSocket();

                                return;
                            }
                        }
                        else
                        {
                            session.sendPacket("answer", packetCode, new string[] { "error" });
                        }
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