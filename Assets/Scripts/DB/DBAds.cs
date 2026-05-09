using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace Fulbo.DB
{
    public class DBAds : MonoBehaviour
    {
        public enum PrizeTypes {
            CHEST,
            COINS,
            HARD,
            PLAYER_NORMAL,
            PLAYER_RARE
        }

        public DataFromServer data;

        string today;

        [Serializable]
        public class DataFromServer
        {
            public int adsDisplayed;
            public string[] prizes;
            public string currentPrize;
            public string bigPrize;
            
        }
        public bool IsNull()
        {
            if (data.prizes == null) return true;
            return data.prizes.Length < 1;
        }
        public bool CheckForSecondRound() {
            return data.adsDisplayed == 5;                
        }
        public bool NoMoreAdsForToday()
        {
            return data.adsDisplayed >= 10;
        }
        public void ResetAdsForToday() {
            data.adsDisplayed = 0;
        }
        public int GetActualAd()
        {
            return data.adsDisplayed;
        }
        public void Load(System.Action OnSuccess)
        {
            StartCoroutine(LoadDataC(OnSuccess));
        }
        IEnumerator LoadDataC(System.Action OnSuccess)
        {
            WWWForm form = new WWWForm();
            string url = DBManager.Instance.URL + "users/" + DBManager.Instance.Email + "/dailyAds/displayed";

            UnityWebRequest www = UnityWebRequest.Get(url);

            print("[GET] " + url);

            yield return www.SendWebRequest();

            if (www.isNetworkError)
                Debug.LogError(string.Format("{0}: {1}", www.url, www.error));
            else
            {
                string s = www.downloadHandler.text;
                Debug.Log(s);
                data = JsonUtility.FromJson<DataFromServer>(s);
                if (NoMoreAdsForToday()) {
                    SetNoMoreAds();
                }
                if (OnSuccess != null)
                {
                    OnSuccess();
                    OnSuccess = null;
                }
                Debug.Log(string.Format("Response: {0}", www.downloadHandler.text));
            }
        }


        [Serializable]
        class DataToDB
        {
            public string device;
            public string version;
            public string hash;
        }
        [Serializable]
        public class PrizeData
        {
            public string prizeType;
            public int coins;
            public int characterId;
            public string role;
            public string rarity;

            public int hard;
            public int energy;
            public int shard;
        }
        System.Action<PrizeData> OnSetNewAdWatchedDone;
        public void SetNewAdWatched(System.Action<PrizeData> OnSetNewAdWatchedDone)
        {
            this.OnSetNewAdWatchedDone = OnSetNewAdWatchedDone;
            string url = DBManager.Instance.URL + "users/" + DBManager.Instance.Email + "/dailyAds/displayed";
            print("Set New Ad Watched: " + url);

            DBUserData uData = DBManager.Instance.DbUserData;
            string hashString =
                DBManager.Instance.Email + DBManager.HASH_SALT1;

            DataToDB d = new DataToDB();
            d.device = Application.platform.ToString();
            d.version = Application.version;
            d.hash = Utils.SHA(hashString);

            string json = JsonUtility.ToJson(d, true);
            DBManager.Instance.Request(url, json, OnSetNewAdWatchedSuccess, "POST", "Updating Set New Ad Watched");
        }
        void OnSetNewAdWatchedSuccess(bool isOk, string result)
        {
            data.adsDisplayed++;
            if (NoMoreAdsForToday()) {
                SetNoMoreAds();
            }
            PrizeData pdata = null;
            Debug.Log("OnSetNewAdWatchedSuccess " + result);
            try {
                pdata = JsonUtility.FromJson<PrizeData>(result);
            } catch (Exception e) {
                Debug.LogError(e.Message);
            }
            OnSetNewAdWatchedDone(pdata);
        }

        void SetNoMoreAds() {
            today = Utils.Today(DB.DBManager.Instance.versionMode == DB.DBManager.versionModes.PROD);
            Events.OnNoMoreAdsForToday();
            Events.OnFreeStaffUpdate(Fulbo.UI.Shop.Shop.sectionType.DAILY_REWARDS, false);
        }

        public bool IsANewDay() {
            bool newDay = Utils.Today(DB.DBManager.Instance.versionMode == DB.DBManager.versionModes.PROD) != today;
            if (newDay)
                DB.DBManager.Instance.DbAds.ResetAdsForToday();
            return newDay;
        }
    }
}
