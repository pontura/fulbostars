using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace Fulbo.DB
{
    public class DBCoins
    {  
        [Serializable]
        class CoinsDB
        {
            public string purchase_id;
            public string device;
            public string version;
            public string hash;
        }

        [Serializable]
        class CoinSoftDB
        {
            public string device;
            public string version;
            public string hash;
        }
        //purchase_id(purchase id from the store)
        //device
        //version
        //hash: email + hashSalt1 + id + coins + purchase_id(coins is how many coins is in the pack)

        public void BuySoft(int id, System.Action<bool, string> OnSuccess)
        {
            string url;
            url = DBManager.Instance.UrlCoinsData + id;

            DBUserData uData = DBManager.Instance.DbUserData;
            string hashString =
                DBManager.Instance.Email +
                "coins" +
                DBManager.HASH_SALT1 +
                id;

            CoinSoftDB d = new CoinSoftDB();
            d.device = Application.platform.ToString();
            d.version = Application.version;
            d.hash = Utils.SHA(hashString);

            string json = JsonUtility.ToJson(d, true);
            DBManager.Instance.Request(url, json, OnSuccess, "POST", "Updating Coins");
        }
        public void Save(int packID, string purchase_id, System.Action<bool, string> OnSuccess)
        {
            string url = DBManager.Instance.UrlCoinsHardData + packID;

            DBUserData uData = DBManager.Instance.DbUserData;
            string hashString =
                DBManager.Instance.Email + 
                "hard" + 
                DBManager.HASH_SALT1 +
                packID +
                purchase_id;

            CoinsDB d = new CoinsDB();
            d.purchase_id = purchase_id;
            d.device = Application.platform.ToString();
            d.version = Application.version;

            d.hash = Utils.SHA(hashString);

            string json = JsonUtility.ToJson(d, true);
            DBManager.Instance.Request(url, json, OnSuccess, "POST", "Updating Coins");
        }
    }
}
