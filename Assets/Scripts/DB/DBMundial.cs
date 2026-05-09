using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace Fulbo.DB
{
    public class DBMundial : MonoBehaviour
    {
        public DataFromServer data;
        [Serializable]
        public class DataFromServer
        {
            public ResultsData[] results;
        }
        [Serializable]
        public class ResultsData
        {
            public string country;
            public int users;
            public int base_score;

            //Local scores:
            public string user_id;
            public string user;
            public int matches;
            public string discord;
            public string twitter;
        }
        public ResultsData GetRankingFor(string country)
        {
            foreach(ResultsData r in data.results)
            {
                if (r.country == country)
                    return r;
            }
            return null;
        }
        public void Load(System.Action<DataFromServer> OnSuccess)
        {
            print(OnSuccess);
            StartCoroutine(LoadDataC(OnSuccess));
        }
        public void LoadLocal(string country, System.Action<DataFromServer> OnSuccess)
        {
            print(OnSuccess);
            StartCoroutine(LoadDataC(OnSuccess, country));
        }
        IEnumerator LoadDataC(System.Action<DataFromServer> OnSuccess, string countryName = "")
        {
            WWWForm form = new WWWForm();
            string url = DBManager.Instance.URL + "ranking/country";
            if (countryName != "")
                url += "/" + countryName;

            UnityWebRequest www = UnityWebRequest.Get(url);

            print("[GET] " + url);

            yield return www.SendWebRequest();

            if (www.isNetworkError)
                Debug.LogError(string.Format("{0}: {1}", www.url, www.error));
            else
            {
                string s = www.downloadHandler.text;
                Debug.Log(s);

                if (countryName != "")
                {
                    data = JsonUtility.FromJson<DataFromServer>(s);
                    OnSuccess(data);
                }
                else
                {
                    OnSuccess(JsonUtility.FromJson<DataFromServer>(s));
                }

                Debug.Log(string.Format("Response: {0}", www.downloadHandler.text));
            }
        }



        public void LoadMyScore(System.Action<ResultsData> OnSuccess)
        {
            StartCoroutine(LoadMyScoreC(OnSuccess));
        }
        IEnumerator LoadMyScoreC(System.Action<ResultsData> OnSuccess)
        {
            WWWForm form = new WWWForm();
            string url = DBManager.Instance.URL + "users/" + DBManager.Instance.Email + "/country";

            UnityWebRequest www = UnityWebRequest.Get(url);

            print("[GET] " + url);

            yield return www.SendWebRequest();

            if (www.isNetworkError)
                Debug.LogError(string.Format("{0}: {1}", www.url, www.error));
            else
            {
                string s = www.downloadHandler.text;

                OnSuccess(JsonUtility.FromJson<ResultsData>(s));                

                Debug.Log(string.Format("Response: {0}", www.downloadHandler.text));
            }
        }
    }
}
