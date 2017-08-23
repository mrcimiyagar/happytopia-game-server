using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midopia.HappytopiaServer.Models
{
    public class GameStartReqHolder
    {
        private GameStartReq currentRequest;
        public GameStartReq CurrentRequest { get { return this.currentRequest; } set { this.currentRequest = value; } }
    }
}