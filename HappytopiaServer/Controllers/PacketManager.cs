using Midopia.HappytopiaServer.Functions;
using Midopia.HappytopiaServer.Functions.Archer;
using Midopia.HappytopiaServer.Functions.Main;
using Midopia.HappytopiaServer.Models;
using Midopia.HappytopiaServer.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Midopia.HappytopiaServer.Controllers
{
    class PacketManager
    {
        private Dictionary<string, BaseFunc> functionsDic;
        
        public PacketManager()
        {
            this.functionsDic = new Dictionary<string, BaseFunc>();

            this.functionsDic.Add("register", new Register());
            this.functionsDic.Add("login", new Login());
            this.functionsDic.Add("logout", new Logout());
            this.functionsDic.Add("fetch_my_user_info", new FetchMyUserInfo());
            this.functionsDic.Add("on_trash_collected", new OnTrashCollected());
            this.functionsDic.Add("on_kababi_collected", new OnKababiCollected());
            this.functionsDic.Add("on_plane_game_started", new OnPlaneGameStarted());
            this.functionsDic.Add("on_plane_catch_coin", new OnPlaneCatchCoin());
            this.functionsDic.Add("on_plane_record", new OnNewPlaneRecord());
            this.functionsDic.Add("fetch_top_plane_users", new FetchTopPlaneUsers());
            this.functionsDic.Add("buy_view", new BuyView());
            this.functionsDic.Add("on_coin_increased", new OnCoinIncreased());
            this.functionsDic.Add("on_gem_increased", new OnGemIncreased());
            this.functionsDic.Add("on_ad_watched", new OnAdWatched());
            this.functionsDic.Add("on_stone_collected", new OnStoneCollected());
            this.functionsDic.Add("buy_nava", new BuyNava());
            this.functionsDic.Add("get_game_version", new FetchGameVersion());
            this.functionsDic.Add("get_home_views_costs", new GetHomeViewsCosts());
            this.functionsDic.Add("get_special_offer_times", new GetSpecialOfferTimes());
            
            this.functionsDic.Add("archer_start_game", new ArcherStartGame());
            this.functionsDic.Add("archer_end_game", new ArcherEndGame());
            this.functionsDic.Add("archer_shoot_bullet", new ArcherShootBullet());
            this.functionsDic.Add("archer_get_user_info", new ArcherFetchUserInfo());
            this.functionsDic.Add("archer_replay_game", new ArcherPlayAgain());
            this.functionsDic.Add("get_archer_leagues_level", new ArcherGetLeaguesInfo());
        }

        public void process(Session session, string function, long packetCode, string[] args)
        {
            Console.WriteLine("request received : " + function);

            if (function != "keep_alive")
            {
                BaseFunc funcObj = null;

                if (this.functionsDic.TryGetValue(function, out funcObj))
                {
                    try
                    {
                        funcObj.process(session, packetCode, args);
                    }
                    catch (Exception ignored) { Console.WriteLine(ignored.ToString()); }
                }
            }
            else
            {
                session.sendPacket("keep_alive", packetCode, new string[0]);
            }
        }
    }
}