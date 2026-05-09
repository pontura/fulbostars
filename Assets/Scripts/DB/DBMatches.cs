using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace Fulbo.DB
{
    public class DBMatches : MonoBehaviour
    {
        public Data data;
        [Serializable]
        public class Data
        {
            public List<MatchData> results;
        }
        [Serializable]
        public class PlayersThatPlayed
        {
            public List<PlayerData> players;
        }
        [Serializable]
        public class PlayerData
        {
            public int id;
        }

        [Serializable]
        public class MatchData
        {
            public string user;
            public string device;
            public int cup;
            public int tier;
            public int id;
            public int stadium;
            public int level;
            public PlayersThatPlayed stats;
            public int score;
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

            public int max_score;
            public int max_full_score;

            public string Name()
            {
                if(user.Length>13)
                {
                    return user.Substring(0, 13);
                }
                return user;
            }
        }
        private void Start()
        {
            DBEvents.LoadMatches += LoadMatches;
            Events.ResetApp += ResetApp;
        }
        private void OnDestroy()
        {
            DBEvents.LoadMatches -= LoadMatches;
            Events.ResetApp -= ResetApp;
        }
        void ResetApp()
        {
            data.results.Clear();
        }
        void LoadMatches(System.Action OnSuccess)
        {
            OnSuccess();
           /// StartCoroutine(LoadMatchesC(OnSuccess));
        }
        IEnumerator LoadMatchesC(System.Action OnSuccess)
        {
            UnityWebRequest www = UnityWebRequest.Get(DBManager.Instance.UrlLoadMatches);

            yield return www.SendWebRequest();

            if (www.isNetworkError)
                Debug.LogError(string.Format("{0}: {1}", www.url, www.error));
            else
            {
                string s = www.downloadHandler.text;
                Debug.Log(s);

                data = JsonUtility.FromJson<Data>(s);


                if (data != null && OnSuccess != null)
                {
                    OnSuccess();
                }

               //  Debug.Log(string.Format("Response: {0}", www.downloadHandler.text));
            }
        }
        public MatchData GetBestMatchResults(int stadium, int level, bool checkForGoalDifference = false)
        {
            MatchData bestMatch = null;
            foreach (MatchData md in data.results)
            {
                if(md.stadium == stadium && md.level == level)
                {
                    if (bestMatch == null)
                        bestMatch = md;
                    else
                    {
                        if (checkForGoalDifference) //Generalmente compara el score del partido, pero para checkeos de si desbloquear un nivel nuevo compara la diferencia de goles
                        {
                            if (md.score_team2 - md.score_team1 > bestMatch.score_team2 - bestMatch.score_team1)
                                bestMatch = md;
                        }
                        else
                        {
                            if (md.score > bestMatch.score)
                                bestMatch = md;
                        }
                    }
                }
            }
            return bestMatch;
        }
        public List<MatchData> GetLevelsPlayedInStadium(int stadium)
        {
            List<MatchData> levels = new List<MatchData>();
            foreach(MatchData data in data.results)
            {
                if (data.stadium == stadium)
                    levels.Add(data);
            }
            return levels;
        }
        public void Add(MatchData md)
        {
            data.results.Add(md);
        }
    }
}
