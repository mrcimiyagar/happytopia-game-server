using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midopia.HappytopiaServer.Models
{
    public class GameStartReq
    {
        private User requester;
        public User Requester { get { return this.requester; } }

        public GameStartReq(User requester)
        {
            this.requester = requester;
        }

        public void cancelRequest()
        {
            this.requester = null;
        }
    }
}