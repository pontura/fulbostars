using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Fulbo.DB
{
    public class DBBoosts
    {
        [Serializable]
        class StatsData
        {
            public string hash;
        }

        // type = "score" || "xp"
        public void SendBoost(string type, System.Action<bool, string> OnSuccess)
        {
            int match = DB.DBManager.Instance.DbTrack.GetLastMatchTracked();
            string url = DBManager.Instance.URL + "users/" + DBManager.Instance.Email + "/track/" + match + "/boost/" + type;
            Debug.Log("DB Boosts : " + url);

            string hashText =
                DBManager.Instance.Email +
                DBManager.HASH_SALT1 +
                match + type.ToUpper();

            StatsData tData = new StatsData();
            tData.hash = Utils.SHA(hashText);

            string json = JsonUtility.ToJson(tData, true);
            DBManager.Instance.Request(url, json, OnSuccess, "PUT", "Sending Boosts for " + type);
        }
    }
}
