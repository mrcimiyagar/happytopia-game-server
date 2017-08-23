using Midopia.HappytopiaServer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Midopia.HappytopiaServer.Models.Archer
{
    public class ArcherBotGame : ArcherGame
    {
        private long id;
        public long Id { get { return this.id; } set { this.id = value; } }

        private User user;
        public User User { get { return this.user; } set { this.user = value; } }

        private ArcherBot bot;
        public ArcherBot Bot { get { return this.bot; } set { this.bot = value; } }

        private int level;
        public int Level { get { return this.level; } set { this.level = value; } }

        private int userScore;
        public int UserScore { get { return this.userScore; } set { this.userScore = value; } }

        private int botScore;
        public int BotScore { get { return this.botScore; } set { this.botScore = value; } }

        private int userLevelScore;
        public int UserLevelScore { get { return this.userLevelScore; } set { this.userLevelScore = value; } }

        private int botLevelScore;
        public int BotLevelScore { get { return this.botLevelScore; } set { this.botLevelScore = value; } }

        private int userLeftBullets;
        public int UserLeftBullets { get { return this.userLeftBullets; } set { this.userLeftBullets = value; } }

        private int botLeftBullets;
        public int BotLeftBullets { get { return this.botLeftBullets; } set { this.botLeftBullets = value; } }

        private float userFinalShoot;
        public float UserFinalShoot { get { return this.userFinalShoot; } set { this.userFinalShoot = value; } }

        private float botFinalShoot;
        public float BotFinalShoot { get { return this.botFinalShoot; } set { this.botFinalShoot = value; } }

        private System.Timers.Timer levelTimer;
        public System.Timers.Timer LevelTimer { get { return this.levelTimer; } }

        private bool isEnded = false;
        public bool IsEnded { get { return this.isEnded; } set { this.isEnded = value; } }

        private bool userRematch = false;
        public bool UserRematch { get { return this.userRematch; } set { this.userRematch = value; } }

        private bool botRematch = false;
        public bool BotRematch { get { return this.botRematch; } set { this.botRematch = value; } }

        private System.Timers.Timer recycleTimer;
        public System.Timers.Timer RecycleTimer { get { return this.recycleTimer; } }

        private int gameLeagueLevel;
        public int GameLeagueLevel { get { return this.gameLeagueLevel; } }

        public ArcherBotGame(int id)
        {
            try {
                this.id = id;
                this.user = null;
                this.bot = null;
                this.level = 1;
                this.userScore = 0;
                this.botScore = 0;
                this.userLevelScore = 0;
                this.botLevelScore = 0;
                this.userLeftBullets = 3;
                this.botLeftBullets = 3;
                this.userFinalShoot = -1;
                this.botFinalShoot = -1;
                this.isEnded = true;

                this.levelTimer = new System.Timers.Timer(ArcherGame.GameLevelTime);
                this.levelTimer.Elapsed += LevelTimer_Elapsed;
                this.levelTimer.AutoReset = false;

                this.recycleTimer = new System.Timers.Timer(10000);
                this.recycleTimer.Elapsed += RecycleTimer_Elapsed;
                this.recycleTimer.AutoReset = false;
            }
            catch(Exception) { }
        }

        private void LevelTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (this.level < 4)
                {
                    lock (this)
                    {
                        int userLevelScorePreEvaluate = this.UserLevelScore;
                        int botLevelScorePreEvaluate = this.BotLevelScore;

                        this.userLeftBullets = 0;
                        this.botLeftBullets = 0;

                        int preGameLevel = this.level;

                        this.evaluateGame();

                        int gameLevel = this.Level;
                        int userLeftBullets = this.UserLeftBullets;
                        int botLeftBullets = this.BotLeftBullets;
                        int botTScore = this.botScore;
                        int userTScore = this.userScore;
                        int userLevelScore = this.UserLevelScore;
                        int botLevelScore = this.BotLevelScore;

                        this.User.CurrentSession.sendPacket("sub_game_end", -1, new string[] { preGameLevel.ToString(), gameLevel.ToString(), userLevelScore.ToString(), userLeftBullets.ToString(), userScore.ToString(), botLevelScore.ToString(), botLeftBullets.ToString(), botScore.ToString(), userLevelScorePreEvaluate.ToString(), botLevelScorePreEvaluate.ToString() });
                        this.user.CurrentSession.sendPacket("archer_timer_stop", -1, new string[0]);

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

                                this.user.CurrentSession.sendPacket("archer_timer_reset", -1, new string[0]);
                                this.bot.notifyNewSubGameStarted();
                            }
                            catch(Exception ex) { Console.WriteLine(ex.ToString()); }
                        }).Start();

                        handleGameEndDatabase();
                    }
                }
                else
                {
                    int userLevelScorePreEvaluate = this.UserLevelScore;
                    int botLevelScorePreEvaluate = this.BotLevelScore;

                    this.userLeftBullets = 0;
                    this.botLeftBullets = 0;

                    int preGameLevel = this.level;

                    this.evaluateGame();

                    int gameLevel = this.Level;
                    int userLeftBullets = this.UserLeftBullets;
                    int botLeftBullets = this.BotLeftBullets;
                    int botTScore = this.botScore;
                    int userTScore = this.userScore;
                    int userLevelScore = this.UserLevelScore;
                    int botLevelScore = this.BotLevelScore;

                    this.User.CurrentSession.sendPacket("sub_game_end", -1, new string[] { preGameLevel.ToString(), gameLevel.ToString(), userLevelScore.ToString(), userLeftBullets.ToString(), userScore.ToString(), botLevelScore.ToString(), botLeftBullets.ToString(), botScore.ToString(), userLevelScorePreEvaluate.ToString(), botLevelScorePreEvaluate.ToString() });

                    handleGameEndDatabase();
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }

        private void handleGameEndDatabase()
        {
            try {
                if (this.level > 3 && this.userScore != this.botScore)
                {
                    ArcherUser userInfo = null;

                    if (this.UserScore > this.BotScore)
                    {
                        userInfo = this.handleWinnerDatabase(this.gameLeagueLevel, this.user);
                    }
                    else if (this.BotScore > this.UserScore)
                    {
                        userInfo = this.handleLooserDatabase(this.gameLeagueLevel, this.user);
                    }

                    this.end();

                    if (userInfo != null)
                    {
                        this.User.CurrentSession.sendPacket("game_end", -1, new string[] { this.UserScore.ToString(), this.BotScore.ToString(), user.Coin.ToString(), user.Gem.ToString(), user.ArcherLevel.ToString(), userInfo.XP.ToString(), userInfo.Won.ToString(), userInfo.Game.ToString() });
                    }
                    else
                    {
                        this.User.CurrentSession.sendPacket("game_end", -1, new string[] { this.UserScore.ToString(), this.BotScore.ToString() });
                    }
                }
            }
            catch(Exception ex) { Console.WriteLine(ex.ToString()); }
        }

        private void RecycleTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                try
                {
                    this.user.CurrentGame = null;
                }
                catch (Exception ignored) { }

                this.kill(true);

                Program.GameManager.killGame(this);
            }
            catch(Exception ex) { Console.WriteLine(ex.ToString()); }
        }

        public override void userShot(Session session, long packetCode, float y, float z, int score)
        {
            try {
                if (this.Level < 4)
                {
                    if (this.UserLeftBullets > 0)
                    {
                        this.UserLeftBullets--;
                        this.UserLevelScore += score;

                        int userLevelScorePreEvaluate = this.UserLevelScore;
                        int botLevelScorePreEvaluate = this.BotLevelScore;

                        int preGameLevel = this.level;

                        this.evaluateGame();

                        int gameLevel = this.Level;
                        int userLeftBullets = this.UserLeftBullets;
                        int botLeftBullets = this.BotLeftBullets;
                        int userLevelScore = this.UserLevelScore;
                        int botLevelScore = this.BotLevelScore;
                        int userTScore = this.userScore;
                        int botTScore = this.botScore;

                        this.User.CurrentSession.sendPacket("answer", packetCode, new string[] { preGameLevel.ToString(), gameLevel.ToString(), this.UserScore.ToString(), userLevelScore.ToString(), userLeftBullets.ToString(), this.BotScore.ToString(), botLevelScore.ToString(), botLeftBullets.ToString(), score.ToString(), RandHelper.makeRandomFloat(-0.5f, 0.5f).ToString(), RandHelper.makeRandomFloat(-0.5f, 0.5f).ToString() });

                        if (this.UserLeftBullets == 3 && this.BotLeftBullets == 3)
                        {
                            this.User.CurrentSession.sendPacket("sub_game_end", -1, new string[] { preGameLevel.ToString(), gameLevel.ToString(), userLevelScore.ToString(), userLeftBullets.ToString(), userTScore.ToString(), botLevelScore.ToString(), botLeftBullets.ToString(), botTScore.ToString(), userLevelScorePreEvaluate.ToString(), botLevelScorePreEvaluate.ToString() });
                            this.user.CurrentSession.sendPacket("archer_timer_stop", -1, new string[0]);

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

                                    this.user.CurrentSession.sendPacket("archer_timer_reset", -1, new string[0]);
                                    this.bot.notifyNewSubGameStarted();
                                }
                                catch(Exception ex) { Console.WriteLine(ex.ToString()); }
                            }).Start();
                        }
                        else if (this.UserLeftBullets == 0)
                        {
                            this.User.CurrentSession.sendPacket("sub_game_side_end", -1, new string[] { preGameLevel.ToString(), "user1", userLevelScorePreEvaluate.ToString() });
                        }
                        else if (this.BotLeftBullets == 0)
                        {
                            this.User.CurrentSession.sendPacket("sub_game_side_end", -1, new string[] { preGameLevel.ToString(), "user2", botLevelScorePreEvaluate.ToString() });
                        }

                        handleGameEndDatabase();
                    }
                }
                else
                {
                    if (this.UserLeftBullets > 0)
                    {
                        float distance = y;

                        this.userFinalShoot = distance;
                        this.userLeftBullets--;

                        int userLevelScorePreEvaluate = this.UserLevelScore;
                        int botLevelScorePreEvaluate = this.BotLevelScore;

                        int preGameLevel = this.level;

                        this.evaluateGame();

                        int gameLevel = this.Level;
                        int userLeftBullets = this.UserLeftBullets;
                        int botLeftBullets = this.BotLeftBullets;
                        int userLevelScore = this.UserLevelScore;
                        int botLevelScore = this.BotLevelScore;
                        int userTScore = this.userScore;
                        int botTScore = this.botScore;

                        this.User.CurrentSession.sendPacket("answer", packetCode, new string[] { preGameLevel.ToString(), gameLevel.ToString(), this.UserScore.ToString(), userLevelScore.ToString(), userLeftBullets.ToString(), this.BotScore.ToString(), botLevelScore.ToString(), botLeftBullets.ToString(), score.ToString() });

                        handleGameEndDatabase();
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }

        public void botShot(int score, int angle)
        {
            try {
                if (this.BotLeftBullets > 0)
                {
                    this.BotLeftBullets--;
                    this.BotLevelScore += score;

                    int userLevelScorePreEvaluate = this.UserLevelScore;
                    int botLevelScorePreEvaluate = this.BotLevelScore;

                    int preGameLevel = this.level;

                    this.evaluateGame();

                    int gameLevel = this.Level;
                    int userLeftBullets = this.UserLeftBullets;
                    int botLeftBullets = this.BotLeftBullets;
                    int userLevelScore = this.UserLevelScore;
                    int botLevelScore = this.BotLevelScore;
                    int userTScore = this.userScore;
                    int botTScore = this.botScore;

                    this.User.CurrentSession.sendPacket("bot_shoot", -1, new string[] { preGameLevel.ToString(), gameLevel.ToString(), this.UserScore.ToString(), userLevelScore.ToString(), userLeftBullets.ToString(), this.BotScore.ToString(), botLevelScore.ToString(), botLeftBullets.ToString(), score.ToString(), angle.ToString() });

                    if ((this.level < 4 && this.UserLeftBullets == 3 && this.BotLeftBullets == 3) || (this.level >= 4 && (this.UserLeftBullets == 1 && this.BotLeftBullets == 1) || (this.UserLeftBullets == 0 && this.BotLeftBullets == 0)))
                    {
                        this.User.CurrentSession.sendPacket("sub_game_end", -1, new string[] { preGameLevel.ToString(), gameLevel.ToString(), userLevelScore.ToString(), userLeftBullets.ToString(), userTScore.ToString(), botLevelScore.ToString(), botLeftBullets.ToString(), botTScore.ToString(), userLevelScorePreEvaluate.ToString(), botLevelScorePreEvaluate.ToString() });
                        this.user.CurrentSession.sendPacket("archer_timer_stop", -1, new string[0]);

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

                                this.user.CurrentSession.sendPacket("archer_timer_reset", -1, new string[0]);
                                this.bot.notifyNewSubGameStarted();
                            }
                            catch(Exception ex) { Console.WriteLine(ex.ToString()); }
                        }).Start();
                    }
                    else if (this.UserLeftBullets == 0)
                    {
                        this.User.CurrentSession.sendPacket("sub_game_side_end", -1, new string[] { preGameLevel.ToString(), "user1", userLevelScorePreEvaluate.ToString() });
                    }
                    else if (this.BotLeftBullets == 0)
                    {
                        this.User.CurrentSession.sendPacket("sub_game_side_end", -1, new string[] { preGameLevel.ToString(), "user2", botLevelScorePreEvaluate.ToString() });
                    }

                    handleGameEndDatabase();
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }

        public void botFinalShot(float singleScore, int angle)
        {
            try {
                if (this.botLeftBullets > 0)
                {
                    float distance = singleScore;

                    this.botFinalShoot = distance;
                    this.botLeftBullets--;

                    int preGameLevel = this.level;

                    this.evaluateGame();

                    int gameLevel = this.Level;
                    int userLeftBullets = this.UserLeftBullets;
                    int botLeftBullets = this.BotLeftBullets;
                    int userLevelScore = this.UserLevelScore;
                    int botLevelScore = this.BotLevelScore;

                    this.User.CurrentSession.sendPacket("bot_shoot", -1, new string[] { preGameLevel.ToString(), gameLevel.ToString(), this.UserScore.ToString(), userLevelScore.ToString(), userLeftBullets.ToString(), this.BotScore.ToString(), botLevelScore.ToString(), botLeftBullets.ToString(), singleScore.ToString(), angle.ToString() });

                    handleGameEndDatabase();
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }

        private void evaluateGame()
        {
            try {
                if (this.level < 4)
                {
                    if (this.UserLeftBullets == 0 && this.BotLeftBullets == 0)
                    {
                        if (this.UserLevelScore > this.BotLevelScore)
                        {
                            this.UserScore++;
                        }
                        else if (this.BotLevelScore > this.UserLevelScore)
                        {
                            this.BotScore++;
                        }

                        if (this.level < 3)
                        {
                            this.UserLevelScore = 0;
                            this.BotLevelScore = 0;
                            this.UserLeftBullets = 3;
                            this.BotLeftBullets = 3;

                            this.Level++;
                        }
                        else if (this.level == 3)
                        {
                            this.UserLevelScore = 0;
                            this.BotLevelScore = 0;
                            if (this.userScore == this.botScore)
                            {
                                this.UserLeftBullets = 1;
                                this.BotLeftBullets = 1;
                            }
                            this.bot.notifyFinalSubGameStarted();
                            this.Level++;
                        }

                        this.resetTimer();
                    }
                }
                else
                {
                    if (this.userLeftBullets == 0 && this.botLeftBullets == 0)
                    {
                        this.level++;

                        if (this.userFinalShoot == -1)
                        {
                            this.botScore++;
                        }
                        else if (this.botFinalShoot == -1)
                        {
                            this.UserScore++;
                        }
                        else
                        {
                            if (this.userFinalShoot < this.BotFinalShoot)
                            {
                                this.UserScore++;
                            }
                            else if (this.userFinalShoot > this.BotFinalShoot)
                            {
                                this.botScore++;
                            }
                            else
                            {
                                if (this.userScore == this.botScore)
                                {
                                    this.UserLeftBullets = 1;
                                    this.BotLeftBullets = 1;
                                }
                                this.bot.notifyFinalSubGameStarted();
                                this.UserFinalShoot = -1;
                                this.BotFinalShoot = -1;
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }

        public override void start(int leagueLevel)
        {
            try
            {
                this.gameLeagueLevel = leagueLevel;

                this.handleUserGameStartDatabase(this.gameLeagueLevel, this.user);

                this.gameLeagueLevel = leagueLevel;
                this.isEnded = false;
                this.userRematch = false;
                this.botRematch = false;

                user.CurrentSession.sendPacket("game_start", -1, new string[] { bot.Id.ToString(), bot.Name, "user1", RandHelper.makeRandomFloat(-0.5f, 0.5f).ToString(), RandHelper.makeRandomFloat(-0.5f, 0.5f).ToString() });

                new Thread(() =>
                {
                    try {
                        Thread.Sleep(5000);
                        this.levelTimer.Start();
                        user.CurrentSession.sendPacket("archer_timer_reset", -1, new string[0]);
                        this.bot.startBot(this);
                        this.bot.notifyNewSubGameStarted();
                    }
                    catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                }).Start();
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }

        public void end()
        {
            try
            {
                if (!this.isEnded)
                {
                    this.levelTimer.Stop();
                    this.isEnded = true;
                    this.recycleTimer.Start();

                    new System.Threading.Thread(() =>
                    {
                        try {
                            System.Threading.Thread.Sleep(4000);

                            int botChoice = RandHelper.makeRandom(0, 2);

                            if (botChoice == 0)
                            {
                                this.botRematch = true;
                            }
                            else
                            {
                                this.botRematch = false;
                                this.notifyBotLeftGame();
                                this.kill(false);
                            }
                        }
                        catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                    }).Start();
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }

        public void kill(bool notifyUser)
        {
            try
            {
                this.recycleTimer.Stop();

                if (notifyUser)
                {
                    try
                    {
                        this.user.CurrentSession.sendPacket("game_force_end", -1, new string[0]);
                    }
                    catch (Exception ignored) { }
                }

                this.bot.stopBot();

                Program.BotManager.killBot(this.bot);

                this.user.CurrentGame = null;
                this.user = null;
                this.bot = null;
                this.level = 1;
                this.userScore = 0;
                this.botScore = 0;
                this.userLevelScore = 0;
                this.botLevelScore = 0;
                this.userLeftBullets = 3;
                this.botLeftBullets = 3;
                this.userFinalShoot = -1;
                this.botFinalShoot = -1;
                this.isEnded = false;

                this.levelTimer.Stop();
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }

        public override void restart()
        {
            try
            {
                this.recycleTimer.Stop();

                this.level = 1;
                this.userScore = 0;
                this.botScore = 0;
                this.userLevelScore = 0;
                this.botLevelScore = 0;
                this.userLeftBullets = 3;
                this.botLeftBullets = 3;
                this.userFinalShoot = -1;
                this.botFinalShoot = -1;
                this.isEnded = false;

                this.start(gameLeagueLevel);
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }

        public void resetTimer()
        {
            try
            {
                this.levelTimer.Stop();
                this.levelTimer.Start();
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }

        public override void gameForceEnd(User user)
        {
            try
            {
                Program.DatabaseManager.decreaseUserCoin(user.Id, 100);
                user.Coin -= 100;

                this.levelTimer.Stop();

                this.kill(false);
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }

        public void notifyBotLeftGame()
        {
            try
            {
                this.user.CurrentSession.sendPacket("archer_op_left", -1, new string[0]);
            }
            catch(Exception ignored) { Console.WriteLine(ignored.ToString()); }
        }

        public override void clearPlayers()
        {
            try
            {
                this.user = null;
                this.bot = null;
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }
    }
}