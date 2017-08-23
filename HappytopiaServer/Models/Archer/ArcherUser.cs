using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midopia.HappytopiaServer.Models.Archer
{
    public class ArcherUser
    {
        private int xp;
        public int XP { get { return this.xp; } }

        private int game;
        public int Game { get { return this.game; } }

        private int won;
        public int Won { get { return this.won; } }

        public ArcherUser(int xp, int game, int won)
        {
            this.xp = xp;
            this.game = game;
            this.won = won;
        }
    }
}