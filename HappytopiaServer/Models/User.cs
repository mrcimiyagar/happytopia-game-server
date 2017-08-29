using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Midopia.HappytopiaServer.Models
{
    public class User : Player
    {
        private int id;
        public int Id { get { return this.id; } }

        private string email;
        public string Email { get { return this.email; } set { this.email = value; } }

        private string password;
        public string Password { get { return this.password; } }

        private string name;
        public string Name { get { return this.name; } }

        private int coin;
        public int Coin
        {
            get { return this.coin; }
            set
            {
                this.coin = value;
                if (this.coin < 0)
                {
                    this.coin = 0;
                }
            }
        }

        private int gem;
        public int Gem
        {
            get { return this.gem; }
            set
            {
                this.gem = value;
                if (this.gem < 0)
                {
                    this.gem = 0;
                }
            }
        }
        
        public int StarsCount
        {
            get
            {
                int count = 0;

                foreach(bool b in this.views)
                {
                    if (b)
                    {
                        count++;
                    }
                }

                if (count >= 9) return 8;
                else if (count >= 7) return 5;
                else if (count >= 6) return 3;
                else if (count >= 4) return 2;
                else return 1;
            }
        }

        private bool isOnline;
        public bool IsOnline { get { return this.isOnline; } set { this.isOnline = value; } }

        private Session currentSession;
        public Session CurrentSession { get { return this.currentSession; } set { this.currentSession = value; } }

        private GameStartReq gameStartReq;
        public GameStartReq GameStartReq { get { return this.gameStartReq; } set { this.gameStartReq = value; } }

        private Game currentGame;
        public Game CurrentGame { get { return this.currentGame; } set { this.currentGame = value; } }

        private long planeRecord;
        public long PlaneRecord { get { return this.planeRecord; } set { this.planeRecord = value; } }

        private int archerLevel;
        public int ArcherLevel { get { return this.archerLevel; } set { this.archerLevel = value; } }

        private int shootballLevel;
        public int ShootballLevel { get { return this.shootballLevel; } set { this.shootballLevel = value; } }

        private bool[] views;
        public bool[] Views { get { return this.views; } set { this.views = value; } }

        private bool[] navas;
        public bool[] Navas { get { return this.navas; } set { this.navas = value; } }

        public User(int id, string email, string password, string name)
        {
            this.id = id;
            this.email = email;
            this.password = password;
            this.name = name;
            this.coin = 0;
            this.gem = 0;
            this.isOnline = false;
            this.gameStartReq = null;
            this.currentGame = null;
            this.planeRecord = 0;
            this.archerLevel = 1;
            this.shootballLevel = 1;
            this.views = new bool[11];
            for (int counter = 0; counter < 11; counter++)
            {
                this.views[counter] = false;
            }
            this.navas = new bool[4];
            for (int counter = 0; counter < 4; counter++)
            {
                this.navas[counter] = false;
            }
        }
    }
}