using AoE2Clan.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoE2Clan.DB
{
    class Leaderboard
    {
        private static string urlParameters = "?leaderboard_id=3&search=";
        private static string urlParameters2 = "?leaderboard_id=4&search=";
        private CommunicationApi communicationApi = new CommunicationApi();

        public string getLeaderBoardInfo(bool isRankTG, string ClanToCheck)
        {
            string message = "";
            string urlToCheck = urlParameters;
            if (isRankTG)
            {
                urlToCheck = urlParameters2;
            }

            dynamic obj = communicationApi.GetDataFromAPI(requestType.leaderboard, urlToCheck + ClanToCheck + "&count=10000");

            if (obj != null)
            {
                var allUser = ((Newtonsoft.Json.Linq.JArray)obj.leaderboard);
                int count = 1;

                var tasks = new List<Task>();
                foreach (var singleUser in allUser)
                {
                    if (communicationApi.checkClan(ClanToCheck, singleUser["clan"].ToString(), singleUser["name"].ToString()))
                    {
                        var task = Task.Factory.StartNew(() => communicationApi.OnlineCheckAsync(singleUser["steam_id"].ToString()));
                        tasks.Add(task);
                    }
                }
                Task.WaitAll(tasks.ToArray());

                List<OnlineInfo> onlineInfoList = new List<OnlineInfo>();
                foreach (Task task in tasks)
                {
                    var result = ((Task<Task<OnlineInfo>>)task).Result;
                    try
                    {
                        if (result != null && result.Result != null)
                        {
                            onlineInfoList.Add(result.Result);
                        }
                    }
                    catch (AggregateException AggregateException)
                    {
                        continue;
                    }
                }

                foreach (var singleUser in allUser)
                {
                    if (communicationApi.checkClan(ClanToCheck, singleUser["clan"].ToString(), singleUser["name"].ToString()))
                    {
                        string SteamID = singleUser["steam_id"].ToString();

                        string dotOnline = "🔴";
                        if (SteamID != "")
                        {
                            OnlineInfo actualInfo = onlineInfoList.Where(s => s.SteamId == SteamID).FirstOrDefault();
                            if (actualInfo != null && actualInfo.status)
                            {
                                dotOnline = "🟢";
                            }
                        }

                        string name = singleUser["name"].ToString();
                        int pos = name.IndexOf("(twitch");
                        if (pos >= 0)
                        {
                            name = name.Remove(pos);
                        }

                        name = name.Replace("twitch.tv/", "");

                        if (name.Length > 25)
                        {
                            name = name.Substring(0, 22) + ".. ";
                        }

                        message += dotOnline + " #" + count + " " + name + ": " + singleUser["rating"].ToString() + "\n";
                        count++;
                    }
                }
            }
            return message;
        }
    }
}
