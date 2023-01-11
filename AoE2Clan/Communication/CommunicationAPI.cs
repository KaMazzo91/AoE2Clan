using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AoE2Clan.Communication
{
    class CommunicationApi
    {
        private const string GenericUrl = "https://aoe2.net/api/";
        private static string urlSteam = "https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v2/";
        
        //TODO da mettere in envairment
        private static string urlSteamParameter = "?key=" + Environment.GetEnvironmentVariable("SteamKey") + "&format=json&steamids=";

        private string GetPartialUrlFromRequestType(requestType requestType)
        {
            switch (requestType)
            {
                case requestType.genericData:
                    return "strings";
                case requestType.leaderboard:
                    return "leaderboard";
                case requestType.ratinghistory:
                    return "player/ratinghistory";
                case requestType.matches:
                    return "player/matches";
                default:
                    return "";
            }
        }

        public dynamic GetDataFromAPI(requestType requestType, string requestParameter)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(GenericUrl + GetPartialUrlFromRequestType(requestType));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync(requestParameter).Result;

                if (response.IsSuccessStatusCode)
                {
                    var resultData = response.Content.ReadAsStringAsync().Result;

                    if(requestType == requestType.matches || requestType == requestType.ratinghistory)
                    {
                        return JArray.Parse(resultData);
                    }

                    return JObject.Parse(resultData);
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        public bool checkClan(string ClanToCheck, string ActualName, string userName)
        {
            if (ClanToCheck.ToUpper() == "GDR" && (userName == "GdR_Scugo's favourite blamed guy" || userName == "GdR_scià"))
            {
                return true;
            }

            if (ActualName.ToUpper().Contains(ClanToCheck.ToUpper()) && ((ClanToCheck.ToUpper() == "MLT" && ActualName.ToUpper() != "MLTS") || ClanToCheck.ToUpper() != "MLT"))
            {
                if (ClanToCheck.ToUpper() != "OS" || (ClanToCheck.ToUpper() == "OS" && ActualName.ToUpper() == "XOSX"))
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<OnlineInfo> OnlineCheckAsync(string SteamID, string xBoxId = "")
        {
            OnlineInfo onlineInfo = new OnlineInfo();
            onlineInfo.SteamId = SteamID;
            onlineInfo.xBoxId = xBoxId;
            onlineInfo.status = false;

            if (SteamID != "")
            {
                HttpClient clientSteam = new HttpClient();
                clientSteam.BaseAddress = new Uri(urlSteam);
                clientSteam.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

                // List data response.
                HttpResponseMessage responseSteam = clientSteam.GetAsync(urlSteamParameter + SteamID).Result;
                if (responseSteam.IsSuccessStatusCode)
                {
                    var resultData2 = responseSteam.Content.ReadAsStringAsync().Result;
                    dynamic obj2 = JObject.Parse(resultData2);
                    onlineInfo.status = Convert.ToBoolean(obj2.response.players[0]["personastate"].Value);
                }
            }

            return onlineInfo;
        }
    }


    enum requestType
    {
        genericData = 0,
        leaderboard = 1,
        ratinghistory = 2,
        matches = 3
    }
}

//Deprecato il metodo Join
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
