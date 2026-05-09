using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;
using System.Collections.Generic;
using static Fulbo.DB.DBMatches;

namespace Fulbo.DB
{
    public class DBStats
    {

        [Serializable]
        class StatsData
        {
            public string hash;
            public string device;
            public string version;
        }

        public void Upload(int characterID, Settings.stat stat, string url, System.Action<bool, string> OnSuccess)
        {
            Debug.Log("DBStats Upload : " + url);
            WWWForm form = new WWWForm();
            string device = Application.platform.ToString();

            string hashText =
                DBManager.Instance.Email +
                DBManager.HASH_SALT1 +
                characterID +
                stat.ToString();

            StatsData tData = new StatsData();
            tData.device = device;
            tData.version = Application.version;
            tData.hash = Utils.SHA(hashText);

            string json = JsonUtility.ToJson(tData, true);
            DBManager.Instance.Request(url, json, OnSuccess, "PUT", Data.Instance.texts.Get("http_sending_scores"));

        }
    }
}
