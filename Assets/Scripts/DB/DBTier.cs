using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace Fulbo.DB
{
    public class DBTier : MonoBehaviour
    {
        [Serializable]
        class DataDB
        {
            public string device;
            public string version;
            public string hash;
        }
        public void Save(DBUserData.DBCharacterData uData, System.Action<bool, string> OnSuccess)
        {
            string url = DBManager.Instance.URL + "users/" + DBManager.Instance.Email + "/characters/" + uData.id + "/tier";
            print("SAVE Tier: " + url);

            int id = uData.id;
            int nextTier = uData.tier + 1;

            string hashString = 
                DBManager.Instance.Email + "upgrade_tier" + DBManager.HASH_SALT1 + id + nextTier;

            DataDB d = new DataDB();
            d.device = Application.platform.ToString();
            d.version = Application.version;
            d.hash = Utils.SHA(hashString);

            string json = JsonUtility.ToJson(d, true);
            DBManager.Instance.Request(url, json, OnSuccess, "PUT", "Updating Tier");
        }

    }
}
