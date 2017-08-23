using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using Midopia.HappytopiaServer.Models;
using System.IO;
using Midopia.HappytopiaServer.Models.Archer;
using Excel;

namespace Midopia.HappytopiaServer.Controllers
{
    class DatabaseManager
    {
        // shahrdari : 5
        // tirkaman : 10
        // kababi : 300
        // nava : 1000
        // shop : 50
        // eyvan : 10000
        // credits : 3000
        // mashinbazi : 
        // leaderboards : 
        // hotels : 30000
        // clock : 
        // garden : 
        // flowerbox :

        public static int[] HomeViewsCosts = new int[] { 5, 10, 300, 1000, 50, 10000, 3000, 1000000, 1000000, 30000, 1000000, 1000000, 1000000 };

        public static int[] SpecialOfferTimes = new int[] { 50, 13 };

        private string usersDBPath = @"UsersDB";
        private string archerDBPath = @"ArcherDB";
        private string shootballDBPath = @"ShootballDB";

        private SQLiteConnection usersDBCon;
        private SQLiteConnection archersDBCon;
        private SQLiteConnection shootballDBCon;

        private string[] botNamesArr;

        public DatabaseManager()
        {
            // create necessary database files

            if (!File.Exists(usersDBPath))
            {
                SQLiteConnection.CreateFile(usersDBPath);
            }

            if (!File.Exists(archerDBPath))
            {
                SQLiteConnection.CreateFile(archerDBPath);
            }

            if (!File.Exists(shootballDBPath))
            {
                SQLiteConnection.CreateFile(shootballDBPath);
            }

            if (!File.Exists("GameVersion.txt"))
            {
                File.WriteAllText("GameVersion.txt", "1");
            }

            // ***

            // config databases and create necessary files

            usersDBCon = new SQLiteConnection(@"Data Source=" + usersDBPath + ";Version=3;");
            usersDBCon.Open();
            SQLiteCommand command = new SQLiteCommand("create table if not exists Users (id integer primary key autoincrement, email varchar(50), password varchar(64), name varchar(50), coin integer, gem integer, plane_record bigint, archer_level integer, shootball_level integer);", usersDBCon);
            command.ExecuteNonQuery();
            SQLiteCommand command0 = new SQLiteCommand("create table if not exists Views (id integer primary key, v0 integer, v1 integer, v2 integer, v3 integer, v4 integer, v5 integer, v6 integer, v7 integer, v8 integer, v9 integer, v10 integer);", usersDBCon);
            command0.ExecuteNonQuery();
            SQLiteCommand command3 = new SQLiteCommand("create table if not exists Navas (id integer primary key, n0 integer, n1 integer, n2 integer, n3 integer);", usersDBCon);
            command3.ExecuteNonQuery();

            archersDBCon = new SQLiteConnection(@"Data Source=" + archerDBPath + ";Version=3;");
            archersDBCon.Open();
            SQLiteCommand command1 = new SQLiteCommand("create table if not exists Archers (id integer primary key, xp integer, game integer, won integer);", archersDBCon);
            command1.ExecuteNonQuery();

            shootballDBCon = new SQLiteConnection(@"Data Source=" + shootballDBPath + ";Version=3;");
            shootballDBCon.Open();
            SQLiteCommand command2 = new SQLiteCommand("create table if not exists Shootball (id integer primary key, xp integer, game integer, won integer);", shootballDBCon);
            command2.ExecuteNonQuery();

            // load bots names array
            botNamesArr = readExcelFile(@"Bot Names.xlsx");

            // mehdi
            /*SQLiteCommand commandVIP = new SQLiteCommand("update Users set coin = 100000 where name = 'fsca';", usersDBCon);
            commandVIP.ExecuteNonQuery();*/
        }

        public string getGameVersion()
        {
            return File.ReadAllText("GameVersion.txt");
        }
        
        public User createUser(string password, string name)
        {
            int id = -1;
            lock (usersDBCon)
            {
                SQLiteCommand command1 = new SQLiteCommand("insert into Users (email, password, name, coin, gem, plane_record, archer_level, shootball_level) values ('not_set', '" + password + "', '" + name + "', 60000, 60000, 0, 1, 1);", usersDBCon);
                command1.ExecuteNonQuery();
                SQLiteCommand command2 = new SQLiteCommand("select last_insert_rowid()", usersDBCon);
                id = Convert.ToInt32(command2.ExecuteScalar());
            }
            SQLiteCommand command0 = new SQLiteCommand("insert into Views (id, v0, v1, v2, v3, v4, v5, v6, v7, v8, v9, v10) values (" + id + ", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);", usersDBCon);
            command0.ExecuteNonQuery();
            SQLiteCommand command5 = new SQLiteCommand("insert into Navas (id, n0, n1, n2, n3) values (" + id + ", 1, 0, 0, 0);", usersDBCon);
            command5.ExecuteNonQuery();
            SQLiteCommand command3 = new SQLiteCommand("insert into Archers (id, xp, game, won) values (" + id + ", 0, 0, 0)", archersDBCon);
            command3.ExecuteNonQuery();
            SQLiteCommand command4 = new SQLiteCommand("insert into Shootball (id, xp, game, won) values (" + id + ", 0, 0, 0)", shootballDBCon);
            command4.ExecuteNonQuery();
            User user = new User(id, "not_set", password, name);
            user.Navas[0] = true;
            user.Coin = 60000;
            user.Gem = 60000;
            return user;
        }

        public User createUser(string email, string password, string name)
        {
            int id = -1;
            lock (usersDBCon)
            {
                SQLiteCommand command1 = new SQLiteCommand("insert into Users (email, password, name, coin, gem, archer_level, shootball_level) values ('" + email + "', '" + password + "', '" + name + "', 0, 0, 1, 1);", usersDBCon);
                command1.ExecuteNonQuery();
                SQLiteCommand command2 = new SQLiteCommand("select last_insert_rowid()", usersDBCon);
                id = Convert.ToInt32(command2.ExecuteScalar());
            }
            SQLiteCommand command3 = new SQLiteCommand("insert into Archers (id, xp, game, won) values (" + id + ", 0, 0, 0)", archersDBCon);
            command3.ExecuteNonQuery();
            SQLiteCommand command4 = new SQLiteCommand("insert into Shootball (id, xp, game, won) values (" + id + ", 0, 0, 0)", shootballDBCon);
            command4.ExecuteNonQuery();
            User user = new User(id, email, password, name);
            return user;
        }

        public List<User> getUsersList()
        {
            List<User> users = new List<User>();
            
            SQLiteCommand command = new SQLiteCommand("select * from Users", usersDBCon);
            SQLiteDataReader r = command.ExecuteReader();

            while (r.Read())
            {
                User user = new User(Convert.ToInt32(r["id"]), r["email"].ToString(), r["password"].ToString(), r["name"].ToString());
                user.Coin = Convert.ToInt32(r["coin"]);
                user.Gem = Convert.ToInt32(r["gem"]);
                user.PlaneRecord = Convert.ToInt64(r["plane_record"]);
                user.ArcherLevel = Convert.ToInt32(r["archer_level"]);
                user.ShootballLevel = Convert.ToInt32(r["shootball_level"]);

                SQLiteCommand command2 = new SQLiteCommand("select * from Views where id = " + user.Id, usersDBCon);
                SQLiteDataReader r2 = command2.ExecuteReader();

                r2.Read();

                for (int counter = 0; counter < 11; counter++)
                {
                    int isEnabled = Convert.ToInt32(r2["v" + counter]);
                    user.Views[counter] = (isEnabled == 1);
                }

                SQLiteCommand command3 = new SQLiteCommand("select * from NAvas where id = " + user.Id, usersDBCon);
                SQLiteDataReader r3 = command3.ExecuteReader();

                r3.Read();

                for (int counter = 0; counter < 4; counter++)
                {
                    int isEnabled = Convert.ToInt32(r3["n" + counter]);
                    user.Navas[counter] = (isEnabled == 1);
                }

                users.Add(user);
            }
            return users;
        }

        public void increaseUserCoin(int id, int coin)
        {
            SQLiteCommand command = new SQLiteCommand("update Users set coin = coin + " + coin + " where id = " + id, usersDBCon);
            command.ExecuteNonQuery();
        }

        public void increaseUserGem(int id, int gem)
        {
            SQLiteCommand command = new SQLiteCommand("update users set gem = gem + " + gem + " where id = " + id, usersDBCon);
            command.ExecuteNonQuery();
        }

        public void decreaseUserCoin(int id, int coin)
        {
            SQLiteCommand command = new SQLiteCommand("update Users set coin = case when (coin - " + coin + ") >= 0 then (coin - " + coin + ") else 0 end where id = " + id, usersDBCon);
            command.ExecuteNonQuery();
        }

        public void decreaseUserGem(int id, int gem)
        {
            SQLiteCommand command = new SQLiteCommand("update users set gem = case when (gem - " + gem + ") >= 0 then (gem - " + gem + ") else 0 end where id = " + id, usersDBCon);
            command.ExecuteNonQuery();
        }

        public void enableHomeView(int userId, int index)
        {
            SQLiteCommand command = new SQLiteCommand("update Views set v" + index + " = 1 where id = " + userId, usersDBCon);
            command.ExecuteNonQuery();
        }

        public void enableNavaMode(int userId, int index)
        {
            SQLiteCommand command = new SQLiteCommand("update Navas set n" + index + " = 1 where id = " + userId, usersDBCon);
            command.ExecuteNonQuery();
        }

        // archer game

        public void increaseArcherUserXP(int id, int addedXP)
        {
            SQLiteCommand command = new SQLiteCommand("update Archers set xp = xp + " + addedXP + " where id = " + id, archersDBCon);
            command.ExecuteNonQuery();
        }

        public void increaseArcherUserLevel(int id)
        {
            SQLiteCommand command = new SQLiteCommand("update users set archer_level = archer_level + 1 where id = " + id, usersDBCon);
            command.ExecuteNonQuery();
        }

        public void increaseArcherUserGameWon(int id)
        {
            SQLiteCommand command = new SQLiteCommand("update archers set game = game + 1, won = won + 1 where id = " + id, archersDBCon);
            command.ExecuteNonQuery();
        }

        public void increaseArcherUserGamePlayed(int id)
        {
            SQLiteCommand command = new SQLiteCommand("update archers set game = game + 1 where id = " + id, archersDBCon);
            command.ExecuteNonQuery();
        }

        public ArcherUser getArcherUserInfo(int id)
        {
            SQLiteCommand command = new SQLiteCommand("select * from Archers where id = " + id, archersDBCon);
            SQLiteDataReader reader = command.ExecuteReader();

            reader.Read();

            ArcherUser archerUser = new ArcherUser(Convert.ToInt32(reader["xp"]), Convert.ToInt32(reader["game"]), Convert.ToInt32(reader["won"]));

            return archerUser;
        }

        public void closeDBConnection()
        {
            usersDBCon.Close();
        }

        public string getRandomBotName()
        {
            if (this.botNamesArr.Length > 0)
            {
                return this.botNamesArr[new Random().Next(this.botNamesArr.Length)];
            }
            else
            {
                return "Bot";
            }
        }

        public void insertNewPlaneRecord(int userId, long scoreRecord)
        {
            SQLiteCommand command = new SQLiteCommand("update Users set plane_record = case when " + scoreRecord + " > plane_record then " + scoreRecord + " else plane_record end where id = " + userId, usersDBCon);
            command.ExecuteNonQuery();
        }

        private string[] readExcelFile(string path)
        {
            try
            {
                List<string> namesList = new List<string>();

                foreach (var worksheet in Workbook.Worksheets(path))
                    foreach (var row in worksheet.Rows)
                        foreach (var cell in row.Cells)
                        {
                            namesList.Add(cell.Text);
                        }

                return namesList.ToArray();
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); return new string[0]; }
        }
    }
}