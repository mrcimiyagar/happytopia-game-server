using Midopia.HappytopiaServer.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Midopia.HappytopiaServer.Controllers
{
    class SharingManager
    {
        private Dictionary<int, User> usersDic;
        public Dictionary<int, User> UsersDic { get { return this.usersDic; } }

        private SortedList<long, HashSet<int>> planePlayersRecords;
        public SortedList<long, HashSet<int>> PlanePlayersRecords { get { return this.planePlayersRecords; } }

        private SortedList<long, HashSet<int>> archersPlayersXPs;
        public SortedList<long, HashSet<int>> ArchersPlayersXPs { get { return this.archersPlayersXPs; } }

        private BlockingCollection<GameStartReq>[] archerPendingUsers;
        public BlockingCollection<GameStartReq>[] ArcherPendingUsers { get { return this.archerPendingUsers; } }

        public SharingManager()
        {
            this.usersDic = new Dictionary<int, User>();

            foreach (User user in Program.DatabaseManager.getUsersList())
            {
                usersDic.Add(user.Id, user);
            }

            this.planePlayersRecords = new SortedList<long, HashSet<int>>(new DescendingCompararer());

            foreach(User user in usersDic.Values)
            {
                if (user.PlaneRecord > 0)
                {
                    Console.WriteLine("User Plane Record : " + user.PlaneRecord);

                    if (!this.planePlayersRecords.ContainsKey(user.PlaneRecord))
                    {
                        HashSet<int> subList = new HashSet<int>();
                        subList.Add(user.Id);
                        this.planePlayersRecords.Add(user.PlaneRecord, subList);
                    }
                    else
                    {
                        this.planePlayersRecords[user.PlaneRecord].Add(user.Id);
                    }
                }
            }

            /*this.archersPlayersXPs = new SortedList<long, HashSet<int>>(new DescendingCompararer());

            foreach (User user in usersDic.Values)
            {
                int userXP = Program.DatabaseManager.getArcherUserInfo(user.Id).XP;

                if (!this.archersPlayersXPs.ContainsKey(userXP))
                {
                    HashSet<int> subList = new HashSet<int>();
                    subList.Add(user.Id);
                    this.planePlayersRecords.Add(userXP, subList);
                }
                else
                {
                    this.planePlayersRecords[userXP].Add(user.Id);
                }
            }*/

            // ***

            this.archerPendingUsers = new BlockingCollection<GameStartReq>[5];

            for (int counter = 0; counter < 5; counter++)
            {
                this.archerPendingUsers[counter] = new BlockingCollection<GameStartReq>();
            }
        }
    }

    class DescendingCompararer : IComparer<long>
    {
        public int Compare(long x, long y)
        {
            // use the default comparer to do the original comparison for datetimes
            int ascendingResult = Comparer<long>.Default.Compare(x, y);

            // turn the result around
            return 0 - ascendingResult;
        }
    }
}