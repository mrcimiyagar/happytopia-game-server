
using Midopia.HappytopiaServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace Midopia.HappytopiaServer.Controllers
{
    public class TGBotManager
    {
        private TelegramBotClient bot;

        private HashSet<string> access = new HashSet<string>()
        {
            "mohammadi_keyhan",
            "shahin_cht",
            "Tahersm",
            "Mohammad_zanjanchi",
            "MehdiRF1",
            "manishabanzadeh",
            "Aras_mehranfar"
        };
        

        public TGBotManager()
        {
            bot = new TelegramBotClient("431189419:AAH8E6Q5w7KQ0OEwVhf6Orj5sux1XITNCpE");
            bot.OnUpdate += this.OnUpdate;
            bot.StartReceiving();

            Console.WriteLine("Happytopia server telegram gate connected.");
        }

        private void OnUpdate(object sender, UpdateEventArgs uea)
        {
            try
            {
                if (uea.Update.Type == Telegram.Bot.Types.Enums.UpdateType.MessageUpdate)
                {
                    if (access.Contains(uea.Update.Message.From.Username))
                    {
                        if (uea.Update.Message.Type == Telegram.Bot.Types.Enums.MessageType.TextMessage)
                        {
                            if (uea.Update.Message.Text != null && uea.Update.Message.Text.Length > 0)
                            {
                                if (uea.Update.Message.Text == "/registered_users_count")
                                {
                                    bot.SendTextMessageAsync(uea.Update.Message.Chat.Id, "There are " + Program.SharingManager.UsersDic.Count + " registered users.");
                                    return;
                                }
                                else if (uea.Update.Message.Text == "/online_users_count")
                                {
                                    bot.SendTextMessageAsync(uea.Update.Message.Chat.Id, "There are " + Program.NetworkManager.SessionsCount + " online users.");
                                    return;
                                }
                                else if (uea.Update.Message.Text.StartsWith("/find_users_by_name "))
                                {
                                    string name = uea.Update.Message.Text.Substring("/find_users_by_name ".Length);

                                    lock (Program.SharingManager.UsersDic)
                                    {
                                        List<User> users = Program.SharingManager.UsersDic.Values.Where<User>(u => u.Name == name).ToList<User>();

                                        if (users.Count > 0)
                                        {
                                            string result = "";

                                            foreach (User user in users)
                                            {
                                                result += user.Name + " with " + user.Coin + " coins found." + Environment.NewLine;
                                            }

                                            bot.SendTextMessageAsync(uea.Update.Message.Chat.Id, "Users :" + Environment.NewLine + result);
                                        }
                                        else
                                        {
                                            bot.SendTextMessageAsync(uea.Update.Message.Chat.Id, "No user found with this name.");
                                        }
                                    }

                                    return;
                                }
                                else if (uea.Update.Message.Text.StartsWith("/award_coin "))
                                {
                                    string data = uea.Update.Message.Text.Substring("/award_coin ".Length);

                                    string[] dataParts = data.Split(' ');

                                    if (dataParts != null && dataParts.Length == 2)
                                    {
                                        string name = dataParts[0];
                                        int coin = Convert.ToInt32(dataParts[1]);

                                        lock (Program.SharingManager.UsersDic)
                                        {
                                            List<User> users = Program.SharingManager.UsersDic.Values.Where<User>(u => u.Name == name).ToList<User>();

                                            if (users.Count > 0)
                                            {
                                                foreach (User user in users)
                                                {
                                                    Program.DatabaseManager.increaseUserCoin(user.Id, coin);
                                                    user.Coin += coin;
                                                }

                                                bot.SendTextMessageAsync(uea.Update.Message.Chat.Id, "Users with the specified name received coin.");
                                            }
                                            else
                                            {
                                                bot.SendTextMessageAsync(uea.Update.Message.Chat.Id, "No user found with this name.");
                                            }
                                        }

                                        bot.SendTextMessageAsync(uea.Update.Message.Chat.Id, "There are " + Program.NetworkManager.SessionsCount + " online users.");
                                    }
                                    else
                                    {
                                        bot.SendTextMessageAsync(uea.Update.Message.Chat.Id, "Wrong command format error.");
                                    }
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception) { }
        }
    }
}