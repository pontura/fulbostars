using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Purchasing.MiniJSON;

namespace Fulbo
{
    public class PricesData : MonoBehaviour
    {
        string url_dev = "https://s3.amazonaws.com/play-testing.fulbogalaxy.com/rewards_and_prices.json";
        string url_prod = "https://s3.amazonaws.com/play.fulbogalaxy.com/rewards_and_prices.json";

        string playerPacks_url = "https://o0fgc808k9.execute-api.us-east-1.amazonaws.com/playersPacks";

        public SimpleJSON.JSONNode jsonNode;

        public SimpleJSON.JSONNode playerPacksJson;

        string today;

        public class TierData
        {
            public float win;
            public float bonus;
        }
        public class SoftData
        {
            public int hard;
            public int soft;
        }
        public class PlayersPackData
        {
            public List<QtySoftHard> pack1;
            public List<QtySoftHard> pack2;
            public List<QtySoftHard> pack3;
            public List<QtySoftHard> pack4;
        }
        public class QtySoftHard
        {
            public int hard;
            public int hardRegular;
            public int hardOnSalePercentage;
            public int soft;
            public int qty;
        }
        private void Start()
        {
            if (Data.Instance.mode == Data.modes.PARTYMODE) return;
            LoadData(null);
            StartCoroutine(GetPlayerPacksPrices());
        }
        public void LoadData(Action OnReady)
        {
            StartCoroutine(GetData(OnReady));
        }
        IEnumerator GetData(System.Action OnReady)
        {
            string url = url_dev;

            if (DB.DBManager.Instance.versionMode == DB.DBManager.versionModes.PROD) url = url_prod;

            using (WWW www = new WWW(url))
            {
                yield return www;
                if (www.error != null)
                {
                    LoadData(OnReady);
                    Events.OnPopup("Connection error. Lets try again...", null);
                }
                else
                {
                    SetData(www.text);
                    if(OnReady != null) OnReady();
                }
            }
        }

        public void SetData(string jsonString) {
            jsonNode = SimpleJSON.JSON.Parse(jsonString);
        }

        IEnumerator GetPlayerPacksPrices() {

            using (WWW www = new WWW(playerPacks_url)) {
                yield return www;
                if (www.error != null) {
                    GetPlayerPacksPrices();
                    Events.OnPopup("Connection error. Lets try again...", null);
                } else {
                 //   playerPacksJson = SimpleJSON.JSON.Parse(www.text);
                    today = Utils.Today(DB.DBManager.Instance.versionMode == DB.DBManager.versionModes.PROD);
                }
            }
        }

        public SimpleJSON.JSONNode GetPlayerPacks(string packKey, string qKey) {
            if (Utils.Today(DB.DBManager.Instance.versionMode == DB.DBManager.versionModes.PROD) != today)
                GetPlayerPacksPrices();
            return playerPacksJson[packKey][qKey];
        }

        public TierData GetScore(int cup, int tierID)
        {
            TierData d = new TierData();
            try {
                string win = jsonNode["rewards"]["matches"][cup.ToString()][tierID.ToString()]["win"];
                string bonus = jsonNode["rewards"]["matches"][cup.ToString()][tierID.ToString()]["bonus"];


                d.win = float.Parse(win);
                d.bonus = float.Parse(bonus);

                print("GetScore cup: " + cup + "   tier:" + tierID + "  win " + d.win + "  bonus " + d.bonus);
            } catch (Exception e) {
                Debug.LogError(e.Message);
            }

            return d;
        }
        public List<int> GetPricesHard() // hardPacks or softPacks
        {
            List<int> arr = new List<int>();

            foreach (string branchKey in jsonNode["prices"]["hardPacks"].Keys)
            {
                int value = int.Parse(jsonNode["prices"]["hardPacks"][branchKey].Value);
                arr.Add(value);
            }
            return arr;
        }
        public List<SoftData> GetPricesSoft() // hardPacks or softPacks
        {
            List<SoftData> arr = new List<SoftData>();

            foreach (string branchKey in jsonNode["prices"]["softPacks"].Keys)
            {
                SoftData sd = new SoftData();
                sd.hard = int.Parse(jsonNode["prices"]["softPacks"][branchKey]["hard"].Value);
                sd.soft = int.Parse(jsonNode["prices"]["softPacks"][branchKey]["soft"].Value);
                arr.Add(sd);
            }
            return arr;
        }

        public int GetUpgradeStatPrice(DB.DBUserData.DBCharacterData characterData) {
            SimpleJSON.JSONNode rarityPrices = jsonNode["players"]["upgrade"][((FigusData.rarities)characterData.rarity).ToString()];
            for(int i= 0; i < 6-characterData.rarity; i++) {
                int tierLimit = 0;
                for (int j = 0; j < 10; j++) tierLimit += int.Parse(rarityPrices["statsPerLevel"][(i * 10) + j]);
                if (characterData.upgraded_stats < tierLimit)
                    return int.Parse(rarityPrices["pricePerStat"][i]);
            }
            return int.Parse(rarityPrices["pricePerStat"][5 - characterData.rarity]);
        }
    }

    
}
