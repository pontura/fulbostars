using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;
using System.Collections.Generic;
using static Fulbo.DB.DBMatches;

namespace Fulbo.DB
{
    public class DBTrack
    {
        int lastMatchTracked;

        [Serializable]
        class TrackData
        {
            public string hash;
            public string device;
            public int cup;
            public int tier;
            public int stadium;
            public int level;
            public int extra_score;
            public int score_team1;
            public int score_team2;
            public int matches;
            public int ball_possesion_team1;
            public int ball_possesion_team2;
            public int kicks_passes_team1;
            public int kicks_passes_team2;
            public int kicks_to_goal_team1;
            public int kicks_to_goal_team2;
            public int balls_to_referi;
            public int centros_team1;
            public int centros_team2;
            public int coins_grabbed;
            public string version;
            public PlayersThatPlayed stats;
        }

        public void Upload(DBMatches.MatchData data, string url, System.Action<bool, string> OnSuccess)
        {
            Debug.Log("Upload : " + url);
            WWWForm form = new WWWForm();
            data.device = Application.platform.ToString();

            lastMatchTracked = data.matches;

            string hashText = 
                DBManager.Instance.Email + 
                data.stadium +
                DBManager.HASH_SALT1 + 
                data.cup + 
                data.tier +
                data.level +
                data.score +
                data.score_team1 +
                data.score_team2 + 
                data.coins_grabbed + 
                DBManager.HASH_SALT2 + 
                data.device + 
                data.matches + 
                data.ball_possesion_team1 +
                data.ball_possesion_team2 + 
                data.kicks_passes_team1 + 
                data.kicks_passes_team2 +
                data.kicks_to_goal_team1 + 
                data.kicks_to_goal_team2 +
                data.balls_to_referi + 
                data.centros_team1 + 
                data.centros_team2 + 
                Application.version +
                JsonUtility.ToJson(data.stats);



                TrackData tData = new TrackData();
                tData.stadium = data.stadium;
                tData.level = data.level;
                tData.cup = data.cup;
                tData.tier = data.tier;
                tData.extra_score = data.score;
                tData.score_team1 = data.score_team1;
                tData.score_team2 = data.score_team2;
                tData.device = data.device;
                tData.matches = data.matches;
                tData.ball_possesion_team1 = data.ball_possesion_team1;
                tData.ball_possesion_team2 = data.ball_possesion_team2;
                tData.kicks_passes_team1 = data.kicks_passes_team1;
                tData.kicks_passes_team2 = data.kicks_passes_team2;
                tData.kicks_to_goal_team1 = data.kicks_to_goal_team1;
                tData.kicks_to_goal_team2 = data.kicks_to_goal_team2;
                tData.balls_to_referi = data.balls_to_referi;
                tData.centros_team1 = data.centros_team1;
                tData.centros_team2 = data.centros_team2;
                tData.coins_grabbed = data.coins_grabbed;
                tData.stats = data.stats;
                tData.version = Application.version;
                tData.hash = Utils.SHA(hashText);

            string json = JsonUtility.ToJson(tData, true);
            DBManager.Instance.Request(url, json, OnSuccess, "POST", Data.Instance.texts.Get("http_sending_scores"));

        }
        public int GetLastMatchTracked() { return lastMatchTracked;  } 
    }
}
