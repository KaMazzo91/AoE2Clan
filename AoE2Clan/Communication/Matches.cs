﻿using AoE2Clan.Communication;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoE2Clan.DB
{
    class Matches
    {
        private static string lastMatchParameters = "?game=aoe2de&profile_id=";
        private static string urlParameters2 = "?leaderboard_id=4&search=";
        private CommunicationApi communicationApi = new CommunicationApi();
        public string getInGameInfo(string ClanToCheck)
        {
            string urlToCheck = urlParameters2;
            dynamic obj = communicationApi.GetDataFromAPI(requestType.leaderboard, urlToCheck + ClanToCheck + "&count=10000");

            if (obj != null)
            {
                var allUser = ((Newtonsoft.Json.Linq.JArray)obj.leaderboard);

                List<string> UserCheked = new List<string>();
                bool gameNotFound = true;
                foreach (var singleUser in allUser)
                {
                    if (communicationApi.checkClan(ClanToCheck, singleUser["clan"].ToString(), singleUser["name"].ToString()))
                    {
                        string SteamID = singleUser["steam_id"].ToString();
                        string ProfileID = singleUser["profile_id"].ToString();

                        dynamic obj2 = communicationApi.GetDataFromAPI(requestType.matches, lastMatchParameters + ProfileID + "&start=0");

                        if (obj2 != null)
                        {
                            string message = "";
                            JArray jsonArray = obj2;

                            string playerId = ProfileID;

                            if (jsonArray.Count() > 0 && jsonArray[0]["finished"] == null)
                            {
                                DateTime startDate = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                                startDate = startDate.AddSeconds((double)jsonArray[0]["started"]);

                                if ((DateTime.Now - startDate.ToLocalTime()).TotalSeconds < 2400)
                                {
                                    if (!UserCheked.Any(s => s == playerId))
                                    {
                                        var players = jsonArray[0]["players"];
                                        message += getGameType(Convert.ToInt32(jsonArray[0]["rating_type_id"])) + ": <b>aoe2de:/1/" + jsonArray[0]["match_id"].ToString() + "</b>\n\n";

                                        int team = 1;
                                        bool firsttimeTeamMinus = true;
                                        int countPlayer = 0;
                                        decimal WinRateTot = 0;

                                        foreach (var player in players.OrderBy(s => Convert.ToInt32(s["team"])))
                                        {
                                            bool playerSmurfAlert = false;
                                            string ratingDefault = "<b>";

                                            int rateTypeId = Convert.ToInt32(jsonArray[0]["leaderboard_id"]);

                                            int firstId = 3;
                                            int secondId = 4;
                                            bool skipFirst = false;
                                            bool skipSecond = false;

                                            if (rateTypeId == 1 || rateTypeId == 2)
                                            {
                                                firstId = 1;
                                                secondId = 2;
                                            }
                                            else if (rateTypeId == 3 || rateTypeId == 4)
                                            {
                                                firstId = 3;
                                                secondId = 4;
                                            }
                                            else if (rateTypeId == 13 || rateTypeId == 14)
                                            {
                                                firstId = 13;
                                                secondId = 14;
                                            }
                                            else if (rateTypeId == 0)
                                            {
                                                firstId = 0;
                                                skipSecond = true;
                                            }
                                            else
                                            {
                                                skipFirst = true;
                                                skipSecond = true;
                                            }

                                            JArray obj3 = null;
                                            JArray obj4 = null;

                                            if (!skipFirst)
                                            {
                                                string urlToCheck3 = "?game=aoe2de&leaderboard_id=" + firstId + "&profile_id=" + player["profile_id"].ToString() + "&count=1";
                                                obj3 = communicationApi.GetDataFromAPI(requestType.ratinghistory, urlToCheck3);

                                                if (obj3 != null)
                                                {
                                                    if (obj3.FirstOrDefault() != null)
                                                    {
                                                        int GameWonS = Convert.ToInt32(obj3.FirstOrDefault()["num_wins"]);
                                                        int GameLostS = Convert.ToInt32(obj3.FirstOrDefault()["num_losses"]);

                                                        if (((GameWonS + GameLostS) >= 10))
                                                        {
                                                            if (GameWonS > 0)
                                                            {
                                                                decimal playerWinRateS = (100 * GameWonS) / (GameWonS + GameLostS);
                                                                if ((GameWonS + GameLostS) <= 100 && playerWinRateS >= 70)
                                                                {
                                                                    playerSmurfAlert = true;
                                                                }
                                                            }
                                                        }

                                                        if (obj3.FirstOrDefault()["rating"].ToString() != "")
                                                        {
                                                            ratingDefault += obj3.FirstOrDefault()["rating"].ToString();
                                                        }
                                                        else
                                                        {
                                                            ratingDefault += "none";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ratingDefault += "none";
                                                    }

                                                    if (!skipSecond)
                                                    {
                                                        ratingDefault += "/";
                                                    }
                                                    else
                                                    {
                                                        ratingDefault += "</b>";
                                                    }
                                                }
                                            }

                                            if (!skipSecond)
                                            {
                                                string urlToCheck4 = "?game=aoe2de&leaderboard_id=" + secondId + "&profile_id=" + player["profile_id"].ToString() + "&count=1";
                                                obj4 = communicationApi.GetDataFromAPI(requestType.ratinghistory, urlToCheck4);

                                                if (obj4 != null)
                                                {
                                                    if (obj4.FirstOrDefault() != null)
                                                    {

                                                        int GameWonS2 = Convert.ToInt32(obj4.FirstOrDefault()["num_wins"]);
                                                        int GameLostS2 = Convert.ToInt32(obj4.FirstOrDefault()["num_losses"]);

                                                        if (((GameWonS2 + GameLostS2) >= 10))
                                                        {
                                                            if (GameWonS2 > 0)
                                                            {
                                                                decimal playerWinRateS2 = (100 * GameWonS2) / (GameWonS2 + GameLostS2);
                                                                if ((GameWonS2 + GameLostS2) <= 100 && playerWinRateS2 >= 70)
                                                                {
                                                                    playerSmurfAlert = true;
                                                                }
                                                            }
                                                        }

                                                        if (obj4.FirstOrDefault()["rating"].ToString() != "")
                                                        {
                                                            ratingDefault += obj4.FirstOrDefault()["rating"].ToString() + " ";
                                                        }
                                                        else
                                                        {
                                                            ratingDefault += "none ";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ratingDefault += "none ";
                                                    }
                                                }

                                                ratingDefault += "</b>";
                                            }

                                            UserCheked.Add(player["profile_id"].ToString());

                                            string name = player["name"].ToString();
                                            int pos = name.IndexOf("(twitch");
                                            if (pos >= 0)
                                            {
                                                name = name.Remove(pos);
                                            }

                                            name = name.Replace("twitch.tv/", "");

                                            int playerTeam = Convert.ToInt32(player["team"]);

                                            if (players.Count() == 2 && playerTeam == -1)
                                            {
                                                playerTeam = team;
                                            }

                                            string alertString = " ";
                                            if (team == playerTeam)
                                            {
                                                if (playerSmurfAlert)
                                                {
                                                    alertString = "❗️";
                                                }

                                                message += getColor(Convert.ToInt32(player["color"])) + alertString + ratingDefault + " " + name + "\n";

                                                if (players.Count() == 2 && Convert.ToInt32(player["team"]) == -1 && firsttimeTeamMinus)
                                                {
                                                    try
                                                    {
                                                        message += "Win Rate: <b>" + Math.Round(WinRateTot / countPlayer, 2) + "%</b>\n";
                                                    }
                                                    catch { }
                                                    finally { countPlayer = 0; WinRateTot = 0; }

                                                    message += "\n VS \n\n";
                                                    firsttimeTeamMinus = false;
                                                }
                                            }
                                            else
                                            {
                                                try
                                                {
                                                    message += "Win Rate: <b>" + Math.Round(WinRateTot / countPlayer, 2) + "%</b>\n";
                                                }
                                                catch { }
                                                finally { countPlayer = 0; WinRateTot = 0; }

                                                message += "\n VS \n\n";
                                                if (playerSmurfAlert)
                                                {
                                                    alertString = "❗️";
                                                }


                                                message += getColor(Convert.ToInt32(player["color"])) + alertString + ratingDefault + " " + name + "\n";
                                                team = playerTeam;
                                            }

                                            if (!skipFirst)
                                            {

                                                JArray objToCheck = null;

                                                if (rateTypeId%2 == 0 && rateTypeId != 0)
                                                {
                                                    objToCheck = obj4;
                                                }
                                                else
                                                {
                                                    objToCheck = obj3;
                                                }

                                                if (objToCheck != null)
                                                {
                                                    if (objToCheck.FirstOrDefault() != null)
                                                    {
                                                        int GameWon = Convert.ToInt32(objToCheck.FirstOrDefault()["num_wins"]);
                                                        int GameLost = Convert.ToInt32(objToCheck.FirstOrDefault()["num_losses"]);

                                                        if (((GameWon + GameLost) >= 10))
                                                        {
                                                            if (GameWon > 0)
                                                            {
                                                                decimal playerWinRate = (100 * GameWon) / (GameWon + GameLost);
                                                                WinRateTot += Math.Round(playerWinRate, 2);

                                                                if ((GameWon + GameLost) <= 100 && playerWinRate >= 70)
                                                                {
                                                                    playerSmurfAlert = true;
                                                                }
                                                            }

                                                            countPlayer++;
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        try
                                        {
                                            message += "Win Rate: <b>" + Math.Round(WinRateTot / countPlayer, 2) + "%</b>";
                                        }
                                        catch { }
                                        finally { countPlayer = 0; }

                                        gameNotFound = false;
                                        return message;
                                    }
                                }
                            }
                        }
                    }
                }


                if (gameNotFound)
                {
                    return "No one is in game!";
                }
                else
                {
                    return "End";
                }
            }

            return "";
        }

        private static string getColor(int colorId)
        {
            switch (colorId)
            {
                case 1:
                    return "🔵";
                case 2:
                    return "🔴";
                case 3:
                    return "🟢";
                case 4:
                    return "🟡";
                case 5:
                    return "🌐";
                case 6:
                    return "🟣";
                case 7:
                    return "⚪️";
                case 8:
                    return "🟠";
                default:
                    return "";
            }
        }

        private static string getGameType(int gameTypeID)
        {
            switch (gameTypeID)
            {
                case 0:
                    return "Unranked";
                case 1:
                    return "1v1 DM ";
                case 2:
                    return "1v1 RM";
                case 3:
                    return "Team DM";
                case 4:
                    return "Team RM";
                case 13:
                    return "1v1 EW";
                case 14:
                    return "Team EW";
                default:
                    return "Unknown";
            }
        }
    }
}