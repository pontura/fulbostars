using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Fulbo.Stadiums;
using Fulbo.Game;
using Fulbo.DB;

namespace Fulbo
{
    public class CupsData : DataLoader
    {
        [HideInInspector] public CupLevelsData levels;

        public bool jumpAutomaticallyToTheMatch;
        public static CupsData mInstance;
        public static CupsData Instance  { get { return mInstance; } }

        public List<CupData> all;

        public List<GameObject> tiersAssets;
        public List<AssetsData> assets;
        [Serializable]
        public class AssetsData
        {
            public GameObject asset;

            public int id; // needs to be equal to the one in the database
        }

        [Serializable]
        public class CupData
        {
            public int id;
            public int tier;
            public string cup_name;
            public int chest;
            public int life;
            public bool available;

            public GameObject GetAssetCup()
            {
                AssetsData assetData = GetAssetsData(id);

                if (assetData != null)
                    return assetData.asset;

                Debug.LogError("No asset for:" + cup_name + "  id:" + id + "  tier:" + tier);
                return GetAssetsData(CupsData.Instance.all[0].id).asset;
            }
          
            AssetsData GetAssetsData(int cupID)
            {
                foreach (AssetsData d in CupsData.Instance.assets)
                    if (d.id == cupID) return d;
                return null;
            }
        }
        public void AddTier(GameObject asset, int tier)
        {
            GameObject tierAsset = GetTierAsset(tier);
            GameObject tierGO = Instantiate(tierAsset, asset.transform);
            tierGO.transform.localPosition = Vector2.zero;
            tierGO.transform.localScale = Vector2.one;
        }
        GameObject GetTierAsset(int tier)
        {
            int id = 1;
            foreach(GameObject go in tiersAssets)
            {
                if (id == tier)
                    return go;
                id++;
            }
            Debug.LogError("No Tier for: " + tier);
            return tiersAssets[0];
        }
        //public void SetAllLevelsReady(int cupID)
        //{
        //    foreach (CupData sData in all)
        //        if (sData.id == cupID)
        //            sData.completed = true;
        //}

        void Awake()
        {
            mInstance = this;
            DontDestroyOnLoad(gameObject);
        }
        Action OnDone;

        public void OnLoad(Action OnDone)
        {
            this.OnDone = OnDone;
            LoadData(null);
        }
        public void Init() // on init
        {
            int cupID = all[0].id;
            DB.DBCupsData cups = DB.DBManager.Instance.DbUserData.data.gameData.cups;
            if (cups  != null && cups.lifesBurned == cups.totalLifes)
                cups.ResetProgress();
        }
        public void LevelsLodaded() // on levels loaded
        {
            //if (DB.DBManager.Instance.DbUserData.data.gameData.cups == null || DB.DBManager.Instance.DbUserData.data.gameData.cups.activeCup == 0)
            //    return;

            //CheckForLocks();

            //int cupID = DB.DBManager.Instance.DbUserData.data.gameData.cups.activeCup;
            //int levelID = 0;
            //if (cupID == 0)
            //{
            //    cupID = all[0].id;
            //    levelID = GetAllLevelsFromCup(cupID)[0].id;
            //}
            //else
            //{
            //    DBCupsData.DBCupLevelData c = DB.DBManager.Instance.DbUserData.data.gameData.cups.GetLastLevelForCup(cupID);
            //    if(c != null)
            //        levelID = c.levelID;
            //}
            //LevelData ld = GetLevelData(cupID, levelID);
            //Data.Instance.matchData.InitLevel(ld);
        }
        public void InitCup(int cupID, int tier)
        {
            DB.DBCupsData cups = DB.DBManager.Instance.DbUserData.data.gameData.cups;
            CupData c = GetCupData(cupID, tier);
            cups.InitCup(c);

            Events.InitCup(cupID, tier);

            Dictionary<string, object> param = new Dictionary<string, object>();
            param["cup"] = cupID;
            param["tier"] = tier;
            Events.OnTrack("InitCup", param);

            ForceOpen(cupID, tier);
        }
        public void EndCup()
        {
            DB.DBCupsData cups = DB.DBManager.Instance.DbUserData.data.gameData.cups;
            cups.activeCup = 0;
            Events.EndCup();
            ForceOpen(0, 0);
            DB.DBManager.Instance.DbUserData.data.gameData.cups.ResetProgress();
        }
        public CupData GetCup(int cupID)
        {
            foreach (CupData sData in all)
                if (sData.id == cupID)
                    return sData;
            Debug.LogError("No cup id: " + cupID);
            return all[0];
        }
        public CupData GetCupData(int id, int tier)
        {
          //  Debug.Log("GetCupName cup assetID: " + id + " tier: " + tier);
            foreach (CupData sData in all)
                if (sData.id == id && sData.tier == tier)
                    return sData;
            Debug.LogError("No cup id: " + id + " tier: " + tier);
            return all[0];
        }
        public List<CupData> GetAllCups()
        {
            CheckForLocks();
            return all;
        }
        public List<CupData> GetTiersForCup(int cupID)
        {
            List<CupData> arr = new List<CupData>();
            foreach (CupData cd in all)
            {
                if (cd.id == cupID)
                    arr.Add(cd);
            }
            return arr;
        }
        public List<CupData> GetCupsTier1()
        {
            List<CupData> arr = new List<CupData>();
            foreach (CupData cd in all)
            {
                if (cd.tier == 1)
                    arr.Add(cd);
            }
            return arr;
        }
        public List<CupData> GetUnlockedCups()
        {
            CheckForLocks();
            List<CupData> unlockeds = new List<CupData>();
            List<CupData> arr = GetCupsTier1();
            foreach (CupData cd in arr)
                unlockeds.Add(GetLastUnlockedCup(cd.id, cd.tier));

            return unlockeds;
        }
        public CupData GetLastUnlockedCup(int cupID, int tier)
        {
            List<DBCupsData.DBCupData> played = DB.DBManager.Instance.DbUserData.data.gameData.cups.cupsPlayed;
        //    print(played + " GetLastUnlockedCup " + cupID + " tier: " + tier);
            if (played != null && played.Count > 0)
            {
                int a = 0;
                DBCupsData.DBCupData unlockedCup = null;
                while (a < played.Count)
                {
                    DBCupsData.DBCupData p = played[a];
                    if (p.cupID == cupID)
                    {
                        if(unlockedCup == null || p.tier > unlockedCup.tier)
                        {
                            unlockedCup = p;
                        }
                    }
                    a++;
                }
                if(unlockedCup != null)
                { 
                   // print("______ new tier placyed for " + unlockedCup.cupID + " tier: " + unlockedCup.tier);
                    return GetCupData(unlockedCup.cupID, unlockedCup.tier);
                }
            }
            return GetCupData(cupID, tier);
        }
        public List<LevelData> GetAllLevelsFromCup(int cupID, int tier)
        {
            List<LevelData> l = levels.GetAllLevelsFromCup(cupID, tier);
            List<LevelData> arr = new List<LevelData>();
            foreach (LevelData ld in l)
                if (ld.cupID == cupID && ld.tier == tier)
                    arr.Add(ld);
            return arr;
        }
        public LevelData GetActualLevel()
        {
            return Data.Instance.matchData.levelData;
        }
        public LevelData GetLevelData(int cupID, int tier, int id)
        {
            List<LevelData> allLevelsInCup = levels.GetAllLevelsFromCup(cupID, tier);
            print("GEt Levels cupID: " + cupID + " tier:" + tier + " id:" + id);
            foreach (LevelData ld in allLevelsInCup)
            {
                if (ld.cupID == cupID && ld.id == id)
                    return ld;
            }
            return null;
        }
        public bool IsLastMatch(int cupID, int tier, int levelID)
        {
            List<LevelData> allLevelsInCup = levels.GetAllLevelsFromCup(cupID, tier);
            if (allLevelsInCup == null || allLevelsInCup.Count == 0) return false;

            print("IsLastMatch? cupID: " + cupID + " levelID: " + allLevelsInCup[allLevelsInCup.Count - 1].id + "  levelID: " + levelID);

            if (allLevelsInCup[allLevelsInCup.Count-1].id == levelID)
                return true;
            return false;
        }
        public override void OnLoaded(List<List<string>> d)
        {
            OnDataLoaded(levels.content, d);
            OnDone();
        }
        void OnDataLoaded(List<LevelData> content, List<List<string>> d)
        {
            int colID = 0;
            int rowID = 0;
            CupData contentLine = null;
            foreach (List<string> line in d)
            {
                foreach (string value in line)
                {
                    //print("row: " + rowID + "  colID: " + colID + "  value: " + value);
                    if (rowID >= 1)
                    {
                        if (colID == 0)
                        {
                            if (value != "")
                            {
                                contentLine = new CupData();

                                contentLine.id = int.Parse(value);
                                all.Add(contentLine);
                            }
                            else
                                return;
                        }
                        else
                        {
                            if (colID == 1 && value != "")
                                contentLine.tier = int.Parse(value);
                            else if (colID == 2 && value != "")
                                contentLine.cup_name = value;
                            else if (colID == 3 && value != "")
                                contentLine.chest = int.Parse(value);
                            else if (colID == 4 && value != "")
                                contentLine.life = int.Parse(value);
                        }
                    }
                    colID++;
                }
                colID = 0;
                rowID++;
            }
            Init();
        }
        void CheckForLocks()
        {
            DB.DBCupsData DBcups = DB.DBManager.Instance.DbUserData.data.gameData.cups;
            if (DBcups == null) return;
            List<DBCupsData.DBCupData> cupsPlayed = DBcups.GetPlayedCupsID(); // played and saved on server:
            int id = 0;
            foreach (CupData cd in all)
            {
                if (id == 0) // first cup:
                {
                    cd.available = true;
                }
                foreach (DBCupsData.DBCupData cupData in cupsPlayed)
                {
                    if (cd.id == cupData.cupID && cd.tier == cupData.tier)
                        cd.available = true;
                }               
                id++;
            }
        }


        CupData wonCup;
        public CupData HasWonTheCup()
        {
            return wonCup;
        }
        //public int GetScoreIfCupWasWon()
        //{
        //    if (wonCup != null)
        //    {
        //        int chestID = wonCup.chest;
        //        ChestsData.ChestData d = ChestsData.Instance.GetChest(chestID);
        //       // Debug.Log("GetScoreIfCupWasWon: " + d.softValue);
        //        return d.softValue;
        //    }
        //    return 0;
        //}
        public void WonCup(int cupID, int tier)
        {
            DB.DBCupsData DBcups = DB.DBManager.Instance.DbUserData.data.gameData.cups;
            DBcups.CupWon(cupID, tier);
            wonCup = GetCupData(cupID, tier);
        }
        public void OnGameOver(System.Action<bool, string> OnReady)
        {
            ForceOpen(Data.Instance.matchData.levelData.cupID, Data.Instance.matchData.levelData.tier);

            if (HasWonTheCup() != null)
            {
                CheckUnlockProgress(OnReady);
                wonCup = null;
            }
            else
                OnReady(true, "Nothing to save");
        }
        void CheckUnlockProgress(System.Action<bool, string> OnReady)
        {
            // CHECK UNLOCK PROGRESS:
            bool doSomething = false;
            DB.DBCupsData DBcups = DB.DBManager.Instance.DbUserData.data.gameData.cups;
            int tier = Data.Instance.matchData.levelData.tier;
            if (tier < 3)
            {
                doSomething = true;
                tier++;
                Debug.Log("Unlock New Tier: " + Data.Instance.matchData.levelData.cupID + " tier " + tier);
                DBcups.UnlockNewCup(Data.Instance.matchData.levelData.cupID, tier); // unlock next tier
                ForceOpen(Data.Instance.matchData.levelData.cupID, tier);
            }
            int cupID = GetNextCup();
            if (cupID != Data.Instance.matchData.levelData.cupID)
            {
                doSomething = true;
                Debug.Log("Unlock New Cup id: " + cupID + " tier 1");
                DBcups.UnlockNewCup(cupID, 1);// unlock next cup tier 1
                DBcups.unlockedCup = true;
                ForceOpen(cupID, 1);
            }
            if (doSomething)
            {
                DB.DBManager.Instance.DbUserData.data.gameData.cups.ResetProgress();
                DB.DBManager.Instance.DbGameData.Put(OnReady);
                Debug.Log("New cup saved: " + cupID);
            }
            else
                OnReady(true, "Nothing to save");
        }
        int GetNextCup()
        {
            int id = 0;
            foreach(CupData d in all)
            {
                if (d.id == Data.Instance.matchData.levelData.cupID && id < all.Count - 1)
                    return all[id + 1].id;
                id++;
            }
            return Data.Instance.matchData.levelData.cupID;
        }


        public int force_open_cupID;
        public int force_open_tier;

        public void ForceOpen(int cupID, int tierID)
        {
            force_open_cupID = cupID;
            force_open_tier = tierID;
        }
         
    }
}