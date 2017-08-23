using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midopia.HappytopiaServer.Models
{
    public class Bot : Player
    {
        private int id;
        public int Id { get { return this.id; } set { this.id = value; } }

        private string name;
        public string Name { get { return this.name; } set { this.name = value; } }
    }
}