using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Midopia.HappytopiaServer.Models;

namespace Midopia.HappytopiaServer.Functions.Main
{
    public class OnNewPlaneRecord : BaseFunc
    {
        public override void process(Session session, long packetCode, string[] args)
        {
            if (session.User != null)
            {
                long scoreRecord = Convert.ToInt64(args[0]);

                Program.DatabaseManager.insertNewPlaneRecord(session.User.Id, scoreRecord);

                lock (Program.SharingManager.PlanePlayersRecords)
                {
                    if (session.User.PlaneRecord < scoreRecord)
                    {
                        long oldScore = session.User.PlaneRecord;
                        session.User.PlaneRecord = scoreRecord;
                        
                        if (Program.SharingManager.PlanePlayersRecords.ContainsKey(scoreRecord))
                        {
                            if (oldScore > 0)
                            {
                                Program.SharingManager.PlanePlayersRecords[oldScore].Remove(session.User.Id);
                            }

                            Program.SharingManager.PlanePlayersRecords[scoreRecord].Add(session.User.Id);

                            if (Program.SharingManager.PlanePlayersRecords[scoreRecord].Count == 0)
                            {
                                Program.SharingManager.PlanePlayersRecords.Remove(scoreRecord);
                            }
                        }
                        else
                        {
                            HashSet<int> subList = new HashSet<int>();
                            subList.Add(session.User.Id);
                            Program.SharingManager.PlanePlayersRecords.Add(scoreRecord, subList);
                        }
                    }
                }

                session.sendPacket("answer", packetCode, new string[0]);
            }
        }
    }
}
