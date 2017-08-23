using Midopia.HappytopiaServer.Utils;
using System;
using System.Threading;
using System.Timers;

namespace Midopia.HappytopiaServer.Models.Archer
{
    public class ArcherUserGame : ArcherGame
    {
        private long id;
        public long Id { get { return this.id; } set { this.id = value; } }

        private User user1;
        public User User1 { get { return this.user1; } set { this.user1 = value; } }

        private User user2;
        public User User2 { get { return this.user2; } set { this.user2 = value; } }

        private int level;
        public int Level { get { return this.level; } set { this.level = value; } }

        private int user1Score;
        public int User1Score { get { return this.user1Score; } set { this.user1Score = value; } }

        private int user2Score;
        public int User2Score { get { return this.user2Score; } set { this.user2Score = value; } }

        private int user1LevelScore;
        public int User1LevelScore { get { return this.user1LevelScore; } set { this.user1LevelScore = value; } }

        private int user2LevelScore;
        public int User2LevelScore { get { return this.user2LevelScore; } set { this.user2LevelScore = value; } }

        private int user1LeftBullets;
        public int User1LeftBullets { get { return this.user1LeftBullets; } set { this.user1LeftBullets = value; } }

        private int user2LeftBullets;
        public int User2LeftBullets { get { return this.user2LeftBullets; } set { this.user2LeftBullets = value; } }

        private float user1FinalShoot;
        public float User1FinalShoot { get { return this.user1FinalShoot; } set { this.user1FinalShoot = value; } }

        private float user2FinalShoot;
        public float User2FinalShoot { get { return this.user2FinalShoot; } set { this.user2FinalShoot = value; } }

        private System.Timers.Timer levelTimer;
        public System.Timers.Timer LevelTimer { get { return this.levelTimer; } }

        private bool isEnded = false;
        public bool IsEnded { get { return this.isEnded; } set { this.isEnded = value; } }

        private bool user1Rematch = false;
        public bool User1Rematch { get { return this.user1Rematch; } set { this.user1Rematch = value; } }

        private bool user2Rematch = false;
        public bool User2Rematch { get { return this.user2Rematch; } set { this.user2Rematch = value; } }

        private System.Timers.Timer recycleTimer;
        public System.Timers.Timer RecycleTimer { get { return this.recycleTimer; } }

        private int gameLeagueLevel;

        public ArcherUserGame(int id)
        {
            try {
                this.id = id;
                this.user1 = null;
                this.user2 = null;
                this.level = 1;
                this.user1Score = 0;
                this.user2Score = 0;
                this.user1LevelScore = 0;
                this.user2LevelScore = 0;
                this.user1LeftBullets = 3;
                this.user2LeftBullets = 3;
                this.user1FinalShoot = -1;
                this.user2FinalShoot = -1;
                this.isEnded = true;

                this.levelTimer = new System.Timers.Timer(ArcherGame.GameLevelTime);
                this.levelTimer.Elapsed += LevelTimer_Elapsed;
                this.levelTimer.AutoReset = false;

                this.recycleTimer = new System.Timers.Timer(10000);
                this.recycleTimer.Elapsed += RecycleTimer_Elapsed;
                this.recycleTimer.AutoReset = false;
            }
            catch(Exception ex) { Console.WriteLine(ex.ToString()); }
        }

        private void LevelTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (this.level < 4)
                {
                    lock (this)
                    {
                        int user1LevelScorePreEvaluate = this.User1LevelScore;
                        int user2LevelScorePreEvaluate = this.User2LevelScore;

                        this.user1LeftBullets = 0;
                        this.user2LeftBullets = 0;

                        int preGameLevel = this.level;

                        this.evaluateGame();

                        int gameLevel = this.Level;
                        int user1TScore = this.user1Score;
                        int user2TScore = this.user2Score;
                        int user1LeftBullets = this.User1LeftBullets;
                        int user2LeftBullets = this.User2LeftBullets;
                        int user1LevelScore = this.User1LevelScore;
                        int user2LevelScore = this.User2LevelScore;

                        this.User1.CurrentSession.sendPacket("sub_game_end", -1, new string[] { preGameLevel.ToString(), gameLevel.ToString(), user1LevelScore.ToString(), user1LeftBullets.ToString(), user1TScore.ToString(), user2LevelScore.ToString(), user2LeftBullets.ToString(), user2TScore.ToString(), user1LevelScorePreEvaluate.ToString(), user2LevelScorePreEvaluate.ToString() });
                        this.User2.CurrentSession.sendPacket("sub_game_end", -1, new string[] { preGameLevel.ToString(), gameLevel.ToString(), user1LevelScore.ToString(), user1LeftBullets.ToString(), user1TScore.ToString(), user2LevelScore.ToString(), user2LeftBullets.ToString(), user2TScore.ToString(), user1LevelScorePreEvaluate.ToString(), user2LevelScorePreEvaluate.ToString() });
                        this.user1.CurrentSession.sendPacket("archer_timer_stop", -1, new string[0]);
                        this.user2.CurrentSession.sendPacket("archer_timer_stop", -1, new string[0]);

                        new Thread(() =>
                        {
                            try {
                                if (this.level < 4)
                                {
                                    Thread.Sleep(2000);
                                }
                                else
                                {
                                    Thread.Sleep(4000);
                                }

                                levelTimer.Start();

                                this.user1.CurrentSession.sendPacket("archer_timer_reset", -1, new string[0]);
                                this.user2.CurrentSession.sendPacket("archer_timer_reset", -1, new string[0]);
                            }
                            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                        }).Start();

                        handleGameEndDatabase();
                    }
                }
                else
                {
                    int user1LevelScorePreEvaluate = this.User1LevelScore;
                    int user2LevelScorePreEvaluate = this.User2LevelScore;

                    this.user1LeftBullets = 0;
                    this.user2LeftBullets = 0;

                    int preGameLevel = this.level;

                    this.evaluateGame();

                    int gameLevel = this.Level;
                    int user1LeftBullets = this.User1LeftBullets;
                    int user2LeftBullets = this.User2LeftBullets;
                    int user1LevelScore = this.User1LevelScore;
                    int user2LevelScore = this.User2LevelScore;

                    this.User1.CurrentSession.sendPacket("sub_game_end", -1, new string[] { preGameLevel.ToString(), gameLevel.ToString(), user1LevelScore.ToString(), user1LeftBullets.ToString(), user2LevelScore.ToString(), user2LeftBullets.ToString(), user1LevelScorePreEvaluate.ToString(), user2LevelScorePreEvaluate.ToString() });
                    this.User2.CurrentSession.sendPacket("sub_game_end", -1, new string[] { preGameLevel.ToString(), gameLevel.ToString(), user1LevelScore.ToString(), user1LeftBullets.ToString(), user2LevelScore.ToString(), user2LeftBullets.ToString(), user1LevelScorePreEvaluate.ToString(), user2LevelScorePreEvaluate.ToString() });

                    handleGameEndDatabase();
                }
            }
            catch(Exception ex) { Console.WriteLine(ex.ToString()); }
        }

        private void RecycleTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try {
                try
                {
                    this.user1.CurrentGame = null;
                }
                catch (Exception ignored) { }

                try
                {
                    this.user2.CurrentGame = null;
                }
                catch (Exception ignored) { }

                this.kill(true);

                Program.GameManager.killGame(this);

                this.recycleTimer.Stop();
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }

        public override void userShot(Session session, long packetCode, float y, float z, int score)
        {
            try
            {
                if (this.Level < 4)
                {
                    if ((this.User1.Id == session.User.Id ? this.User1LeftBullets > 0 : this.User2LeftBullets > 0))
                    {
                        if (this.User1.Id == session.User.Id)
                        {
                            this.User1LeftBullets--;
                            this.User1LevelScore += score;
                        }
                        else
                        {
                            this.User2LeftBullets--;
                            this.User2LevelScore += score;
                        }

                        int user1LevelScorePreEvaluate = this.User1LevelScore;
                        int user2LevelScorePreEvaluate = this.User2LevelScore;

                        int preGameLevel = this.level;

                        this.evaluateGame();

                        int gameLevel = this.Level;
                        int user1TScore = this.user1Score;
                        int user2TScore = this.user2Score;
                        int user1LeftBullets = this.User1LeftBullets;
                        int user2LeftBullets = this.User2LeftBullets;
                        int user1LevelScore = this.User1LevelScore;
                        int user2LevelScore = this.User2LevelScore;

                        if (session.User.Id == this.User1.Id)
                        {
                            this.User2.CurrentSession.sendPacket("user_shoot", -1, new string[] { gameLevel.ToString(), this.User1Score.ToString(), user1LevelScore.ToString(), user1LeftBullets.ToString(), this.User2Score.ToString(), user2LevelScore.ToString(), user2LeftBullets.ToString(), y.ToString(), z.ToString(), score.ToString() });
                            this.User1.CurrentSession.sendPacket("answer", packetCode, new string[] { preGameLevel.ToString(), gameLevel.ToString(), this.User1Score.ToString(), user1LevelScore.ToString(), user1LeftBullets.ToString(), this.User2Score.ToString(), user2LevelScore.ToString(), user2LeftBullets.ToString(), score.ToString(), RandHelper.makeRandomFloat(-0.5f, 0.5f).ToString(), RandHelper.makeRandomFloat(-0.5f, 0.5f).ToString() });
                        }
                        else
                        {
                            this.User1.CurrentSession.sendPacket("user_shoot", -1, new string[] { gameLevel.ToString(), this.User1Score.ToString(), user1LevelScore.ToString(), user1LeftBullets.ToString(), this.User2Score.ToString(), user2LevelScore.ToString(), user2LeftBullets.ToString(), y.ToString(), z.ToString(), score.ToString() });
                            this.User2.CurrentSession.sendPacket("answer", packetCode, new string[] { preGameLevel.ToString(), gameLevel.ToString(), this.User1Score.ToString(), user1LevelScore.ToString(), user1LeftBullets.ToString(), this.User2Score.ToString(), user2LevelScore.ToString(), user2LeftBullets.ToString(), score.ToString(), RandHelper.makeRandomFloat(-0.5f, 0.5f).ToString(), RandHelper.makeRandomFloat(-0.5f, 0.5f).ToString() });
                        }

                        if (this.User1LeftBullets == 3 && this.User2LeftBullets == 3)
                        {
                            this.User1.CurrentSession.sendPacket("sub_game_end", -1, new string[] { preGameLevel.ToString(), gameLevel.ToString(), user1LevelScore.ToString(), user1LeftBullets.ToString(), user1TScore.ToString(), user2LevelScore.ToString(), user2LeftBullets.ToString(), user2TScore.ToString(), user1LevelScorePreEvaluate.ToString(), user2LevelScorePreEvaluate.ToString() });
                            this.User2.CurrentSession.sendPacket("sub_game_end", -1, new string[] { preGameLevel.ToString(), gameLevel.ToString(), user1LevelScore.ToString(), user1LeftBullets.ToString(), user1TScore.ToString(), user2LevelScore.ToString(), user2LeftBullets.ToString(), user2TScore.ToString(), user1LevelScorePreEvaluate.ToString(), user2LevelScorePreEvaluate.ToString() });
                            this.user1.CurrentSession.sendPacket("archer_timer_stop", -1, new string[0]);
                            this.user2.CurrentSession.sendPacket("archer_timer_stop", -1, new string[0]);

                            new Thread(() =>
                            {
                                try {
                                    if (this.level < 4)
                                    {
                                        Thread.Sleep(2000);
                                    }
                                    else
                                    {
                                        Thread.Sleep(4000);
                                    }

                                    levelTimer.Start();

                                    this.user1.CurrentSession.sendPacket("archer_timer_reset", -1, new string[0]);
                                    this.user2.CurrentSession.sendPacket("archer_timer_reset", -1, new string[0]);
                                }
                                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                            }).Start();
                        }
                        else if (this.User1LeftBullets == 0)
                        {
                            this.User1.CurrentSession.sendPacket("sub_game_side_end", -1, new string[] { preGameLevel.ToString(), "user1", user1LevelScorePreEvaluate.ToString() });
                            this.User2.CurrentSession.sendPacket("sub_game_side_end", -1, new string[] { preGameLevel.ToString(), "user1", user1LevelScorePreEvaluate.ToString() });
                        }
                        else if (this.User2LeftBullets == 0)
                        {
                            this.User2.CurrentSession.sendPacket("sub_game_side_end", -1, new string[] { preGameLevel.ToString(), "user2", user2LevelScorePreEvaluate.ToString() });
                            this.User1.CurrentSession.sendPacket("sub_game_side_end", -1, new string[] { preGameLevel.ToString(), "user2", user2LevelScorePreEvaluate.ToString() });
                        }

                        handleGameEndDatabase();
                    }
                }
                else
                {
                    if ((this.User1.Id == session.User.Id ? this.User1LeftBullets > 0 : this.User2LeftBullets > 0))
                    {
                        float distance = y;

                        if (this.user1.Id == session.User.Id)
                        {
                            this.user1FinalShoot = distance;
                            this.user1LeftBullets--;
                        }
                        else
                        {
                            this.user2FinalShoot = distance;
                            this.User2LeftBullets--;
                        }

                        this.evaluateGame();

                        int gameLevel = this.Level;
                        int user1LeftBullets = this.User1LeftBullets;
                        int user2LeftBullets = this.User2LeftBullets;
                        int user1LevelScore = this.User1LevelScore;
                        int user2LevelScore = this.User2LevelScore;

                        if (session.User.Id == this.User1.Id)
                        {
                            this.User2.CurrentSession.sendPacket("user_shoot", -1, new string[] { gameLevel.ToString(), this.User1Score.ToString(), user1LevelScore.ToString(), user1LeftBullets.ToString(), this.User2Score.ToString(), user2LevelScore.ToString(), user2LeftBullets.ToString(), y.ToString(), z.ToString(), score.ToString() });
                            this.User1.CurrentSession.sendPacket("answer", packetCode, new string[] { gameLevel.ToString(), this.User1Score.ToString(), user1LevelScore.ToString(), user1LeftBullets.ToString(), this.User2Score.ToString(), user2LevelScore.ToString(), user2LeftBullets.ToString(), score.ToString() });
                        }
                        else
                        {
                            this.User1.CurrentSession.sendPacket("user_shoot", -1, new string[] { gameLevel.ToString(), this.User1Score.ToString(), user1LevelScore.ToString(), user1LeftBullets.ToString(), this.User2Score.ToString(), user2LevelScore.ToString(), user2LeftBullets.ToString(), y.ToString(), z.ToString(), score.ToString() });
                            this.User2.CurrentSession.sendPacket("answer", packetCode, new string[] { gameLevel.ToString(), this.User1Score.ToString(), user1LevelScore.ToString(), user1LeftBullets.ToString(), this.User2Score.ToString(), user2LevelScore.ToString(), user2LeftBullets.ToString(), score.ToString() });
                        }

                        handleGameEndDatabase();
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }

        private void handleGameEndDatabase()
        {
            try
            {
                if (this.level > 3 && this.User1Score != this.user2Score)
                {
                    string[] resultArgs = new string[] { this.User1Score.ToString(), this.User2Score.ToString() };

                    ArcherUser user1Info = null;
                    ArcherUser user2Info = null;

                    if (this.User1Score > this.User2Score)
                    {
                        user1Info = this.handleWinnerDatabase(this.gameLeagueLevel, this.user1);
                        user2Info = this.handleLooserDatabase(this.gameLeagueLevel, this.user2);
                    }
                    else if (this.User2Score > this.User1Score)
                    {
                        user1Info = this.handleLooserDatabase(this.gameLeagueLevel, this.user1);
                        user2Info = this.handleWinnerDatabase(this.gameLeagueLevel, this.user2);
                    }

                    this.end();

                    if (user1Info != null)
                    {
                        this.User1.CurrentSession.sendPacket("game_end", -1, new string[] { this.user1Score.ToString(), this.user2Score.ToString(), user1.Coin.ToString(), user1.Gem.ToString(), user1.ArcherLevel.ToString(), user1Info.XP.ToString(), user1Info.Won.ToString(), user1Info.Game.ToString() });
                    }
                    else
                    {
                        this.User1.CurrentSession.sendPacket("game_end", -1, new string[] { this.user1Score.ToString(), this.user2Score.ToString() });
                    }

                    if (user2Info != null)
                    {
                        this.User2.CurrentSession.sendPacket("game_end", -1, new string[] { this.user1Score.ToString(), this.user2Score.ToString(), user2.Coin.ToString(), user2.Gem.ToString(), user2.ArcherLevel.ToString(), user2Info.XP.ToString(), user2Info.Won.ToString(), user2Info.Game.ToString() });
                    }
                    else
                    {
                        this.User2.CurrentSession.sendPacket("game_end", -1, new string[] { this.user1Score.ToString(), this.user2Score.ToString() });
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }

        private void evaluateGame()
        {
            try
            {
                if (this.level < 4)
                {
                    if (this.User1LeftBullets == 0 && this.User2LeftBullets == 0)
                    {
                        if (this.User1LevelScore > this.User2LevelScore)
                        {
                            this.User1Score++;
                        }
                        else if (this.User2LevelScore > this.User1LevelScore)
                        {
                            this.User2Score++;
                        }

                        if (this.level < 3)
                        {
                            this.User1LevelScore = 0;
                            this.User2LevelScore = 0;
                            this.User1LeftBullets = 3;
                            this.User2LeftBullets = 3;
                            this.Level++;
                        }
                        else if (this.level == 3)
                        {
                            this.User1LevelScore = 0;
                            this.User2LevelScore = 0;
                            if (this.user1Score == this.user2Score)
                            {
                                this.User1LeftBullets = 1;
                                this.User2LeftBullets = 1;
                            }
                            this.Level++;
                        }

                        this.resetTimer();
                    }
                }
                else
                {
                    if (this.user1LeftBullets == 0 && this.user2LeftBullets == 0)
                    {
                        this.level++;

                        if (this.user1FinalShoot < this.User2FinalShoot)
                        {
                            this.User1Score++;
                        }
                        else if (this.user1FinalShoot > this.User2FinalShoot)
                        {
                            this.user2Score++;
                        }
                        else
                        {
                            if (this.user1Score == this.user2Score)
                            {
                                this.User1LeftBullets = 1;
                                this.User2LeftBullets = 1;
                            }
                            this.User1FinalShoot = -1;
                            this.User2FinalShoot = -1;
                        }
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }

        public override void start(int gameLeagueLevel)
        {
            try {
                this.gameLeagueLevel = gameLeagueLevel;

                this.handleUserGameStartDatabase(this.gameLeagueLevel, this.user1);
                this.handleUserGameStartDatabase(this.gameLeagueLevel, this.user2);

                this.level = 1;
                this.user1Score = 0;
                this.user2Score = 0;
                this.user1LevelScore = 0;
                this.user2LevelScore = 0;
                this.user1LeftBullets = 3;
                this.user2LeftBullets = 3;
                this.user1FinalShoot = -1;
                this.user2FinalShoot = -1;
                this.isEnded = false;
                this.user1Rematch = false;
                this.user2Rematch = false;
                this.levelTimer.Start();

                float xWind = RandHelper.makeRandomFloat(-0.5f, 0.5f);
                float yWind = RandHelper.makeRandomFloat(-0.5f, 0.5f);

                user1.CurrentSession.sendPacket("game_start", -1, new string[] { user2.Id.ToString(), user2.Name, "user1", xWind.ToString(), yWind.ToString() });
                user2.CurrentSession.sendPacket("game_start", -1, new string[] { user1.Id.ToString(), user1.Name, "user2", xWind.ToString(), yWind.ToString() });

                new Thread(() =>
                {
                    try {
                        Thread.Sleep(5000);
                        this.levelTimer.Start();
                        this.user1.CurrentSession.sendPacket("archer_timer_reset", -1, new string[0]);
                        this.user2.CurrentSession.sendPacket("archer_timer_reset", -1, new string[0]);
                    }
                    catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                }).Start();
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }

        public void end()
        {
            try {
                this.levelTimer.Stop();
                this.isEnded = true;
                this.recycleTimer.Start();
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }

        public void kill(bool notifyUser)
        {
            try {
                if (notifyUser)
                {
                    try
                    {
                        this.user1.CurrentSession.sendPacket("game_force_end", -1, new string[0]);
                    }
                    catch (Exception ignored) { }

                    try
                    {
                        this.user2.CurrentSession.sendPacket("game_force_end", -1, new string[0]);
                    }
                    catch (Exception ignored) { }
                }

                this.user1 = null;
                this.user2 = null;
                this.level = 1;
                this.user1Score = 0;
                this.user2Score = 0;
                this.user1LevelScore = 0;
                this.user2LevelScore = 0;
                this.user1LeftBullets = 3;
                this.user2LeftBullets = 3;
                this.user1FinalShoot = -1;
                this.user2FinalShoot = -1;
                this.isEnded = false;

                this.levelTimer.Stop();
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }

        public override void restart()
        {
            try {
                this.recycleTimer.Stop();

                User user1 = this.user2;
                User user2 = this.user1;

                this.user1 = user2;
                this.user2 = user1;
                this.level = 1;
                this.user1Score = 0;
                this.user2Score = 0;
                this.user1LevelScore = 0;
                this.user2LevelScore = 0;
                this.user1LeftBullets = 3;
                this.user2LeftBullets = 3;
                this.user1FinalShoot = -1;
                this.user2FinalShoot = -1;
                this.isEnded = false;

                this.levelTimer.Stop();

                this.start(this.gameLeagueLevel);
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }

        public void resetTimer()
        {
            try {
                this.levelTimer.Stop();
                this.levelTimer.Start();
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }

        public override void gameForceEnd(User user)
        {
            try {
                Program.DatabaseManager.decreaseUserCoin(user.Id, 100);
                user.Coin -= 100;

                this.notifyOpponentLeft(user.Id);

                this.levelTimer.Stop();

                this.kill(false);
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }

        public override void notifyOpponentLeft(int opId)
        {
            try {
                if (opId == this.user1.Id)
                {
                    this.user2.CurrentSession.sendPacket("archer_op_left", -1, new string[0]);
                }
                else
                {
                    this.user1.CurrentSession.sendPacket("archer_op_left", -1, new string[0]);
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }

        public override void clearPlayers()
        {
            try {
                this.user1 = null;
                this.user2 = null;
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }
    }
}