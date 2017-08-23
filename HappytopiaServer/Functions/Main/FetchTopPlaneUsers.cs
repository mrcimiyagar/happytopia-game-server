using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Midopia.HappytopiaServer.Models;

namespace Midopia.HappytopiaServer.Functions.Main
{
    public class FetchTopPlaneUsers : BaseFunc
    {
        public override void process(Session session, long packetCode, string[] args)
        {
            if (session.User != null)
            {
                lock (Program.SharingManager.PlanePlayersRecords)
                {
                    List<KeyValuePair<int, User>> top10UsersList = new List<KeyValuePair<int, User>>();

                    int usersRank = 0;
                    
                    foreach (KeyValuePair<long, HashSet<int>> recordBatches in Program.SharingManager.PlanePlayersRecords)
                    {
                        if (top10UsersList.Count == 10)
                        {
                            break;
                        }

                        usersRank++;

                        foreach (int userId in recordBatches.Value)
                        {
                            if (top10UsersList.Count == 10)
                            {
                                break;
                            }

                            top10UsersList.Add(new KeyValuePair<int, User>(usersRank, Program.SharingManager.UsersDic[userId]));
                        }
                    }

                    string[] resultArr = new string[top10UsersList.Count * 3 + 2];

                    resultArr[0] = session.User.PlaneRecord.ToString();

                    if (session.User.PlaneRecord > 0)
                    {
                        int rank = 0;

                        lock (session.User)
                        {
                            foreach (KeyValuePair<long, HashSet<int>> recordBatches in Program.SharingManager.PlanePlayersRecords)
                            {
                                rank++;

                                if (recordBatches.Key == session.User.PlaneRecord)
                                {
                                    break;
                                }
                            }
                        }
                        
                        resultArr[1] = rank.ToString();
                    }
                    else
                    {
                        resultArr[1] = "0";
                    }

                    int counter = 2;

                    foreach (KeyValuePair<int, User> pair in top10UsersList)
                    {
                        resultArr[counter] = pair.Value.Name;
                        resultArr[counter + 1] = pair.Value.PlaneRecord.ToString();
                        resultArr[counter + 2] = pair.Key.ToString();
                        counter += 3;
                    }

                    session.sendPacket("answer", packetCode, resultArr);
                }
            }
        }
    }
}