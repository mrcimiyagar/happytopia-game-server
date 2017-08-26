using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midopia.HappytopiaServer.Models.Archer
{
    public abstract class ArcherGame : Game
    {
        public static int GameLevelTime = 30000;
        public static int[] StartXpNeed = new int[] { 1000, 10000, 50000, 200000, 1000000 };
        public static int[] StartCoinCost = new int[] { 5, 15, 100, 450, 1500 };
        public static int[] WinCoinPrize = new int[] { 25, 50, 200, 500, 2000 };
        public static int[] WinGemPrize = new int[] { 0, 0, 1, 2, 4 };
        public static int[] WinXpPrize = new int[] { 200, 600, 1500, 3000, 5000 };
        public static int[] LooseXpPrize = new int[] { 100, 300, 750, 1250, 2000 };

        public virtual void userShot(Session session, long packetCode, float y, float z, int score)
        {

        }

        public virtual void start(int gameLevel)
        {

        }

        public virtual void restart()
        {

        }

        public virtual void gameForceEnd(User user)
        {

        }

        public virtual void notifyOpponentLeft(int userId)
        {

        }

        public ArcherUser handleWinnerDatabase(int gameLeagueLevel, User user)
        {
            Program.DatabaseManager.increaseArcherUserGameWon(user.Id);

            if (user.ArcherLevel == gameLeagueLevel)
            {
                Program.DatabaseManager.increaseArcherUserXP(user.Id, WinXpPrize[gameLeagueLevel - 1]);
            }

            Program.DatabaseManager.increaseUserCoin(user.Id, WinCoinPrize[gameLeagueLevel - 1]);
            user.Coin += WinCoinPrize[gameLeagueLevel - 1];

            Program.DatabaseManager.increaseUserGem(user.Id, WinGemPrize[gameLeagueLevel - 1]);
            user.Gem += WinGemPrize[gameLeagueLevel - 1];

            ArcherUser userInfo = Program.DatabaseManager.getArcherUserInfo(user.Id);

            if (userInfo.XP >= StartXpNeed[user.ArcherLevel - 1])
            {
                Program.DatabaseManager.increaseArcherUserLevel(user.Id);
                user.ArcherLevel++;
            }

            return userInfo;
        }

        public ArcherUser handleLooserDatabase(int gameLeagueLevel, User user)
        {
            Program.DatabaseManager.increaseArcherUserGamePlayed(user.Id);

            if (user.ArcherLevel == gameLeagueLevel)
            {
                Program.DatabaseManager.increaseArcherUserXP(user.Id, LooseXpPrize[gameLeagueLevel - 1]);
            }

            ArcherUser userInfo = Program.DatabaseManager.getArcherUserInfo(user.Id);

            if (userInfo.XP >= StartXpNeed[user.ArcherLevel - 1])
            {
                Program.DatabaseManager.increaseArcherUserLevel(user.Id);
                user.ArcherLevel++;
            }

            return userInfo;
        }

        public void handleUserGameStartDatabase(int gameLeagueLevel, User user)
        {
            Program.DatabaseManager.decreaseUserCoin(user.Id, StartCoinCost[gameLeagueLevel - 1]);
            user.Coin -= StartCoinCost[gameLeagueLevel - 1];
        }

        public static int getArcherGameCost(int leagueNum)
        {
            return StartCoinCost[leagueNum - 1];
        }
    }
}