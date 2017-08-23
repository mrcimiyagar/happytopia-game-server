using Midopia.HappytopiaServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midopia.HappytopiaServer.Functions.Main
{
    public abstract class BaseFunc
    {
        public abstract void process(Session session, long packetCode, string[] args);
    }
}