using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace Fulbo.DB
{
    public class DBEnergy : MonoBehaviour
    {       

        [Serializable]
        class EnergyDB
        {
            public string device;
            public string version;
            public string hash;
        }

        EnergyPriceDB data;
        [Serializable]
        public class EnergyPriceDB
        {
            public int count;
            public int price;
        }
        //purchase_id(purchase id from the store)
        //device
        //version
        //hash: email + hashSalt1 + id + coins + purchase_id(coins is how many coins is in the pack)

        public void Save(System.Action<bool, string> OnSuccess)
        {
            string url = DBManager.Instance.UrlEnergyData;
            print("SAVE Energy: " + url);

            DBUserData uData = DBManager.Instance.DbUserData;
            string hashString =
                DBManager.Instance.Email + DBManager.HASH_SALT1;

            EnergyDB d = new EnergyDB();
            d.device = Application.platform.ToString();
            d.version = Application.version;
            d.hash = Utils.SHA(hashString);

            string json = JsonUtility.ToJson(d, true);
            DBManager.Instance.Request(url, json, OnSuccess, "POST", "Updating Energy");
        }

        public EnergyPriceDB GetPrice() {
            return data;
        }
        public IEnumerator GetPriceFromServerCoroutine(System.Action<EnergyPriceDB> OnSuccess)
        {
            UnityWebRequest www = UnityWebRequest.Get(DBManager.Instance.UrlEnergyPriceData);

            yield return www.SendWebRequest();

            if (www.isNetworkError)
                Debug.LogError(string.Format("{0}: {1}", www.url, www.error));
            else
            {
                string s = www.downloadHandler.text;
                Debug.Log(s);

                data = JsonUtility.FromJson<EnergyPriceDB>(s);
               // Debug.Log("data: " + data );
                if (OnSuccess != null)
                    OnSuccess(data);
            }
        }
    }
}
