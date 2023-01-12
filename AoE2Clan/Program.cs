using System;
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
        static Leaderboard leaderboard = new Leaderboard();
        static Matches matches = new Matches();

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

        private static async Task UpdateHandler(ITelegramBotClient bot, Update update, CancellationToken arg3)
        {
            if (update.Type == UpdateType.Message)
            {
                if (update.Message.Type == MessageType.Text)
                {
                    string text = update.Message.Text;
                    long idGroup = update.Message.Chat.Id;

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

                    if ((messageToCheck.Contains("SETCLAN") || messageToCheck.Contains("GETPLAYER")) || checkIfCommand)
                    {
                        if (checkAoeCommand)
                        {
                            CommandAction(messageToCheck, idGroup);
                        }
                        else
                        {
                            _=Bot.SendTextMessageAsync(idGroup, System.Web.HttpUtility.UrlDecode("Command temporarily disabled, due to AOE2.Net under maintenance"));
                        }
                    }
                }
            }
        }

        private static void CommandAction(string messageToCheck, long idGroup)
        {
            bool noClanSet = false;
            string ClanToCheck = ClanInfo.GetClan(idGroup);

            if (messageToCheck == "RANK" || messageToCheck == "RANKTG")
            {
                if (ClanToCheck != "")
                {
                    string message = leaderboard.getLeaderBoardInfo(messageToCheck == "RANKTG", ClanToCheck);
                    _=Bot.SendTextMessageAsync(idGroup, System.Web.HttpUtility.UrlDecode(message));
                }
                else
                {
                    noClanSet = true;
                }
            }
            else if (messageToCheck == "INGAME")
            {
                if (ClanToCheck != null && ClanToCheck != "")
                {
                    matches.getInGameInfo(ClanToCheck, Bot, idGroup);
                }
                else
                {
                    noClanSet = true;
                }
            }
            else if (messageToCheck == "COMMAND")
            {
                string message2 = "- SetClan (Set the clan to monitor. Users must belong to that clan and it must also be present in their name)\n";
                message2 += "- Rank (Show the rank of all members of the clan)\n";
                message2 += "- Ranktg (Show the TG Rank of all members of the clan)\n";
                message2 += "- Ingame (Show matches and opponents of members belonging to the clan!)\n";
                message2 += "- Getplayer (get the given player's Elo information)\n";
                Bot.SendTextMessageAsync(idGroup, System.Web.HttpUtility.UrlDecode(message2));
            }
            else if (messageToCheck.Contains("SETCLAN"))
            {
                string message = "Patch Note 1.2 \n\n";
                message += "Functionality added to the 'ingame' command: \n";
                message += "- The belonging clan is remembered despite future updates. (YEEEEE)\n";
                message += "- the Empire War rating has been added to the 'ingame' command \n";
                message += "- map type and start time added in 'ingame' command \n";
                message += "- Added the 'Getplayer' command \n";

                string ClanName = messageToCheck.Replace("SETCLAN", "").Trim();
                if (ClanToCheck != null && ClanName != "" && ClanToCheck != ClanName)
                {

                    ClanInfo.insertClanInfo(ClanName, idGroup);

                    _=Bot.SendTextMessageAsync(idGroup, "Clan setted");
                    _=Bot.SendTextMessageAsync(idGroup, System.Web.HttpUtility.UrlDecode(message));
                }
                else if (ClanName != "" && ClanToCheck == ClanName)
                {
                    _=Bot.SendTextMessageAsync(idGroup, "Clan setted");
                    _=Bot.SendTextMessageAsync(idGroup, System.Web.HttpUtility.UrlDecode(message));
                }
                else
                {
                    _=Bot.SendTextMessageAsync(idGroup, "Clan not setted");
                }
            }
            else if (messageToCheck == "JOIN")
            {
                //deprecato dal cambio di API di AOE2.NET
            }
            else if (messageToCheck.Contains("GETPLAYER"))
            {
                string playerName = messageToCheck.Replace("GETPLAYER", "").Trim();
                string playerInfo = matches.getPlayerInfo(playerName);
                _=Bot.SendTextMessageAsync(idGroup, System.Web.HttpUtility.UrlDecode(playerInfo), ParseMode.Html);
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

            if (noClanSet)
            {
                _=Bot.SendTextMessageAsync(idGroup, System.Web.HttpUtility.UrlDecode("Before starting, use the setclan command followed by the clan you belong to (e.g. setclan GdR)"));
            }
        }
        private static Task ErrorHandler(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            return null;
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
        JOIN = 3,
        GETPLAYER = 4
    }

    public enum AllCall
    {
        RANK = 0,
        RANKTG = 1,
        INGAME = 2,
        JOIN = 3,
        GETPLAYER = 4,
        PRESCIT = 5,
        COMMAND = 6,
        SETCLAN = 7
    }

}
