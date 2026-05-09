using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace Fulbo.DB
{
    public class DBMatchesPerLevel : MonoBehaviour
    {
        public Data data;
        [Serializable]
        public class Data
        {
            public DBMatches.MatchData[] results;
        }
        private void Start()
        {
            DBEvents.LoadMatchesPerLevel += LoadMatchesPerLevel;
        }
        private void OnDestroy()
        {
            DBEvents.LoadMatchesPerLevel -= LoadMatchesPerLevel;
        }
        void LoadMatchesPerLevel(int stadium, int level, System.Action OnSuccess)
        {
            OnSuccess();
           // StartCoroutine(LoadMatchesPerLevelC(stadium, level, OnSuccess));
        }
        IEnumerator LoadMatchesPerLevelC(int stadium, int level, System.Action OnSuccess)
        {
            string url = DBManager.Instance.UrlLoadMatchesPerLevel + "?from=2022-01-01&stadium=" + stadium  + "&level=" + level + "&limit=5";
            Debug.Log("LoadMatchesPerLevel url: " + url);

            UnityWebRequest www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(data);
                data = JsonUtility.FromJson<Data>(www.downloadHandler.text);
                OnSuccess();
            }
        }
        public DBMatches.MatchData GetBestMatchResults(int stadium, int level)
        {
            DBMatches.MatchData bestMatch = null;
            foreach (DBMatches.MatchData md in data.results)
            {
                if(md.stadium == stadium && md.level == level)
                {
                    if (bestMatch == null)
                        bestMatch = md;
                    else
                    {
                        if(md.score>bestMatch.score)
                            bestMatch = md;
                    }
                }
            }
            return bestMatch;
        }
    }
}
