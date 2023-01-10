﻿using System;
using Telegram.Bot;
using System.Collections.Generic;
using AoE2Clan.DB;
using Telegram.Bot.Types.Enums;
using System.Threading.Tasks;
using System.Threading;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;


namespace AoE2Clan
{
    class Program
    {
        static TelegramBotClient Bot = new TelegramBotClient(Environment.GetEnvironmentVariable("Token"));

        private const bool ErrorAOE2 = false;
        static ClanInfo ClanInfo = new ClanInfo();

        static void Main(string[] args)
        {
            ClanInfo.DbSetup();

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new UpdateType[]
               {
                    UpdateType.Message,
                    UpdateType.EditedMessage,
               }
            };

            Bot.StartReceiving(UpdateHandler, ErrorHandler, receiverOptions);

            Console.ReadLine();
        }

        private static Task ErrorHandler(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            return null;
        }

        private static async Task UpdateHandler(ITelegramBotClient bot, Update update, CancellationToken arg3)
        {
            if (update.Type == UpdateType.Message)
            {
                if (update.Message.Type == MessageType.Text)
                {
                    //write an update
                    string text = update.Message.Text;
                    long idGroup = update.Message.Chat.Id;

                    string ClanToCheck = ClanInfo.GetClan(idGroup);

                    string messageToCheck = text.Replace("/", "");
                    messageToCheck = messageToCheck.ToUpper().Replace("@AOE2CLAN_BOT", "").Trim();

                    AOE2Call aOE2Call;
                    AllCall allCall;
                    bool checkAoeCommand = true;

                    if (ErrorAOE2)
                    {
                        checkAoeCommand = !Enum.TryParse(messageToCheck, out aOE2Call);
                    }

                    bool checkIfCommand = Enum.TryParse(messageToCheck, out allCall);

                    if (messageToCheck.Contains("SETCLAN") || checkIfCommand)
                    {
                        if (checkAoeCommand)
                        {
                            if (messageToCheck == "RANK" || messageToCheck == "RANKTG")
                            {
                                if (ClanToCheck != "")
                                {
                                    Leaderboard leaderboard = new Leaderboard();
                                    string message = leaderboard.getLeaderBoardInfo(messageToCheck == "RANKTG", ClanToCheck);
                                    _=Bot.SendTextMessageAsync(idGroup, System.Web.HttpUtility.UrlDecode(message));
                                }
                                else
                                {
                                    _=Bot.SendTextMessageAsync(idGroup, System.Web.HttpUtility.UrlDecode("Before starting, use the setclan command followed by the clan you belong to (e.g. setclan GdR)"));
                                }
                            }
                            else if (messageToCheck == "INGAME")
                            {
                                if (ClanToCheck != null && ClanToCheck != "")
                                {
                                    Matches matches = new Matches();
                                    string message = matches.getInGameInfo(ClanToCheck);
                                    _=Bot.SendTextMessageAsync(idGroup, System.Web.HttpUtility.UrlDecode(message), ParseMode.Html);
                                }
                                else
                                {
                                    _=Bot.SendTextMessageAsync(idGroup, System.Web.HttpUtility.UrlDecode("Before starting, use the setclan command followed by the clan you belong to (e.g. setclan GdR)"));
                                }
                            }
                            else if (messageToCheck == "COMMAND")
                            {
                                //string message2 = "SetClan (Set the clan to monitor. Users must belong to that clan and it must also be present in their name)\n";
                                //message2 = "Rank (Show the rank of all members of the clan)\n";
                                //message2 += "Ranktg (Show the TG Rank of all members of the clan)\n";
                                //message2 += "games! (work in progress)\n";
                                //message2 += "join (work in progress)\n";
                                //Bot.SendTextMessageAsync(id, System.Web.HttpUtility.UrlDecode(message2));
                            }
                            else if (messageToCheck.Contains("SETCLAN"))
                            {
                                string message = "Patch Note 1.2 \n\n";
                                message += "Functionality added to the 'ingame' command: \n";
                                message += "- The belonging clan is remembered despite future updates. (YEEEEE)\n";
                                message += "- the Empire War rating has been added to the 'ingame' command \n";


                                string ClanName = messageToCheck.Replace("SETCLAN", "").Trim();
                                if (ClanToCheck != null && ClanName != "" && ClanToCheck != ClanName)
                                {

                                    ClanInfo.insertClanInfo(ClanName, idGroup);

                                    _=Bot.SendTextMessageAsync(idGroup, System.Web.HttpUtility.UrlDecode("Clan setted"));

                                    _=Bot.SendTextMessageAsync(idGroup, System.Web.HttpUtility.UrlDecode(message));
                                }
                                else if (ClanName != "" && ClanToCheck == ClanName)
                                {
                                    _=Bot.SendTextMessageAsync(idGroup, System.Web.HttpUtility.UrlDecode("Clan setted"));

                                    _=Bot.SendTextMessageAsync(idGroup, System.Web.HttpUtility.UrlDecode(message));
                                }
                                else
                                {
                                    _=Bot.SendTextMessageAsync(idGroup, System.Web.HttpUtility.UrlDecode("Clan not setted"));
                                }
                            }
                            else if (messageToCheck == "JOIN")
                            {
                                //deprecato nuove API
                                //if (ClanToCheck != null && ClanToCheck != "")
                                //{
                                //    HttpClient client = new HttpClient();
                                //    client.BaseAddress = new Uri(urlPrivateLobbies);
                                //    client.DefaultRequestHeaders.Accept.Add(
                                //    new MediaTypeWithQualityHeaderValue("application/json"));

                                //    HttpResponseMessage response = client.GetAsync("?game=aoe2de").Result;
                                //    if (response.IsSuccessStatusCode)
                                //    {
                                //        var resultData = response.Content.ReadAsStringAsync().Result;
                                //        var allLobbie = JArray.Parse(resultData);

                                //        string lobbieCode = "No lobbie found with name: " + ClanToCheck + " lobbie";

                                //        foreach (var lobbie in allLobbie)
                                //        {
                                //            if (lobbie["name"].ToString().ToUpper().Contains((ClanToCheck + " lobbie").ToUpper()))
                                //            {
                                //                lobbieCode = "aoe2de:/0/" + lobbie["match_id"].ToString();
                                //                break;
                                //            }
                                //        }

                                //        Bot.SendTextMessageAsync(id, System.Web.HttpUtility.UrlDecode(lobbieCode));
                                //    }
                                //}
                                //else
                                //{
                                //    Bot.SendTextMessageAsync(id, System.Web.HttpUtility.UrlDecode("Before starting, use the setclan command followed by the clan you belong to (e.g. setclan GdR)"));
                                //}
                            }
                            else if (messageToCheck == "PRESCIT")
                            {
                                Dictionary<int, string> presscit = new Dictionary<int, string>();
                                presscit.Add(1, "quantoveroiddio me levo dai GdR");
                                presscit.Add(2, "A ragà, dove stanno queste 10000 risorse?");
                                presscit.Add(3, "Madonna becca");
                                presscit.Add(4, "Dio cignialo");
                                presscit.Add(5, "Maledetto il giallo a lui, a chi l'ha inventato e a tutta la Germania");
                                presscit.Add(6, "Taccio!? Taccio!? Dio P***o");
                                presscit.Add(7, "era un 1100 si, ma solido");
                                presscit.Add(8, "oh, non me chiedete più sta sera de giocà mappe che non conosco, che giuro esco dai GdR");
                                presscit.Add(9, "Hera? ma Hera è un pollo, un fuoco de paglia");
                                presscit.Add(10, "Madonna sumara orsa cane, ma guarda te");
                                presscit.Add(11, "Ferma Pier metti pausa, me so dato fuoco alla barba");
                                presscit.Add(12, "E poi me freeza, dio cignale");
                                presscit.Add(13, "You Daut");

                                Random r = new Random();
                                int rInt = r.Next(1, 14); //for ints

                                try
                                {
                                    _=Bot.SendTextMessageAsync(idGroup, System.Web.HttpUtility.UrlDecode(presscit[rInt]));
                                }
                                catch
                                {
                                    _=Bot.SendTextMessageAsync(idGroup, System.Web.HttpUtility.UrlDecode(presscit[1]));
                                }
                            }
                        }
                        else
                        {
                            _=Bot.SendTextMessageAsync(idGroup, System.Web.HttpUtility.UrlDecode("Comando momentanemanete disabilitato, per via di AOE2.Net in manutenzione"));
                        }
                    }
                }
            }
        }
    }

    public class OnlineInfo
    {
        public bool status;
        public string SteamId;
        public string xBoxId;
    }

    public enum AOE2Call
    {
        RANK = 0,
        RANKTG = 1,
        INGAME = 2,
        JOIN = 3
    }

    public enum AllCall
    {
        RANK = 0,
        RANKTG = 1,
        INGAME = 2,
        JOIN = 3,
        PRESCIT = 4,
        COMMAND = 5,
        SETCLAN = 6
    }

}